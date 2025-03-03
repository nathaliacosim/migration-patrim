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

public class EnviarCabecalhoBaixa
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarCabecalhoBaixa(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/baixas");
    }

    public async Task<List<BaixasCabecalho>> BuscarCabecalhoBaixa()
    {
        const string query = "SELECT * FROM baixa_cabecalho_cloud ORDER BY ano, mes, i_motivo;";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<BaixasCabecalho>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar baixa_cabecalho_cloud: {ex.Message}");
            return new List<BaixasCabecalho>();
        }
    }

    public async Task<List<string>> EnviarCabecalhoBaixaCloud()
    {
        var resultado = await BuscarCabecalhoBaixa();
        var lotesIds = new List<string>();

        foreach (var item in resultado)
        {
            var json = JsonConvert.SerializeObject(new BaixaPOST
            {
                tipoBaixa = new TipoBaixaBaixaPOST 
                { 
                    id = item.id_cloud_tipo_baixa 
                },
                dhBaixa = item.dt_baixa + " 00:00:00",
                motivo = item.observacao ?? "NÃO INFORMADO."
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar baixa_cabecalho_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar baixa_cabecalho_cloud {item.id}: {ex.Message}\n");
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
        const string query = "UPDATE baixa_cabecalho_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para baixa_cabecalho_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para baixa_cabecalho_cloud {id}: {ex.Message}");
        }
    }
}
