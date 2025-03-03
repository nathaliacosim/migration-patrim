using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Models.ModelPostgres;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarCabecalhoTransferencia
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarCabecalhoTransferencia(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/transferencia");
    }

    public async Task<List<TransferenciaCabecalho>> BuscarCabecalhoTransferencia()
    {
        const string query = "SELECT * FROM transferencia_cabecalho_cloud ORDER BY ano, mes, tipo_transferencia;";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<TransferenciaCabecalho>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar transferencia_cabecalho_cloud: {ex.Message}");
            return new List<TransferenciaCabecalho>();
        }
    }

    public async Task<int> GetIdCloudOutrasEspeciesAsync(int id_grupo)
    {
        const string query = @"SELECT id_cloud 
                               FROM especie_bem_cloud 
                               WHERE id_grupo_bem = @id_grupo_bem 
                               AND UPPER(descricao) = 'OUTRAS ESPECIES' 
                               AND i_chave = 0";

        try
        {
            using var connection = _pgConnection.GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync<int?>(query, new { id_grupo_bem = id_grupo });

            return result.HasValue ? result.Value : -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar especie_bem_cloud: {ex.Message}");
            return -1;
        }
    }

    public async Task<int> GetIdCloudTipoBemAsync(int id_grupo)
    {
        const string query = @"SELECT id_tipo_bem 
                               FROM grupo_bem_cloud 
                               WHERE id_cloud = @id_cloud;";

        try
        {
            using var connection = _pgConnection.GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync<int?>(query, new { id_cloud = id_grupo.ToString() });

            return result.HasValue ? result.Value : -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar grupo_bem_cloud: {ex.Message}");
            return -1;
        }
    }

    public async Task<List<string>> EnviarCabecalhoTransferenciaCloud()
    {
        var resultado = await BuscarCabecalhoTransferencia();
        var lotesIds = new List<string>();

        foreach (var item in resultado)
        {
            var json = JsonConvert.SerializeObject(new TransferenciaPOST
            {
                dhTransferencia = (item.data_transferencia + " 00:00:00"),
                observacao = item.observacao ?? "",
                nroTransferencia = item.id,
                tipoTransferencia = new TipoTransferenciaTransferenciaPOST
                {
                    id = item.id_tipo_transferencia
                },
                situacao = new SituacaoTransferenciaPOST
                {
                    descricao = "EM_ANDAMENTO",
                    valor = "EM_ANDAMENTO"
                },
                grupoBem = item.tipo_transferencia == "N" ? new GrupoBemTransferenciaPOST
                {
                    id = item.id_cloud_registro
                } : null,
                especieBem = item.tipo_transferencia == "N" ? new EspecieBemTransferenciaPOST
                {
                    id = await GetIdCloudOutrasEspeciesAsync(item.id_cloud_registro)
                } : null,
                tipoBem = item.tipo_transferencia == "N" ? new TipoBemTransferenciaPOST
                {
                    id = await GetIdCloudTipoBemAsync(item.id_cloud_registro)
                } : null,
                responsavel = item.tipo_transferencia == "R" ? new ResponsavelTransferenciaPOST
                {
                    id = item.id_cloud_registro
                } : null,
                organograma = item.tipo_transferencia == "C" || item.tipo_transferencia == "L" ? new OrganogramaTransferenciaPOST
                {
                    id = item.id_cloud_registro
                } : null,
                localizacaoFisica = item.tipo_transferencia == "F" ? new LocalizacaoFisicaTransferenciaPOST
                {
                    id = item.id_cloud_registro
                } : null
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar transferencia_cabecalho_cloud {item.id}: {errorMessage}\n");
                }
                else
                {
                    lotesIds.Add(resposta);
                    Console.WriteLine($"Atualizando ID Cloud: {item.id} -> {resposta}");
                    await AtualizarIdCloud(item.id, resposta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar transferencia_cabecalho_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: transferencia_cabecalho_cloud.\n\n");
        return lotesIds;
    }

    private static bool TryParseErrorResponse(string resposta, out string message)
    {
        try
        {
            var erro = JsonConvert.DeserializeObject<Dictionary<string, string>>(resposta);
            return erro.TryGetValue("message", out message);
        }
        catch (JsonException)
        {
            message = null;
            return false;
        }
    }

    public async Task AtualizarIdCloud(int id, string idCloud)
    {
        const string query = "UPDATE transferencia_cabecalho_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para transferencia_cabecalho_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para transferencia_cabecalho_cloud {id}: {ex.Message}");
        }
    }
}
