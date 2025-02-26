using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarTipoAquisicao
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarTipoAquisicao(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/tipos-aquisicao");
    }

    public async Task<List<Models.ModelPostgres.TipoAquisicao>> BuscarTipoAquisicao()
    {
        const string query = "SELECT * FROM public.tipo_aquisicao_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.TipoAquisicao>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar tipo_aquisicao_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.TipoAquisicao>();
        }
    }

    public async Task<List<string>> EnviarAquisicaoCloud()
    {
        var tipos = await BuscarTipoAquisicao();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new TipoAquisicaoPOST
            {
                descricao = item.descricao,
                classificacao = new ClassificacaoTipoAquisicaoPOST
                {
                    descricao = item.descricao,
                    valor = item.classificacao
                }
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar tipo_aquisicao_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar tipo_aquisicao_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: tipo_aquisicao_cloud.\n\n");
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
        const string query = "UPDATE tipo_aquisicao_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para tipo_aquisicao_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para tipo_aquisicao_cloud {id}: {ex.Message}");
        }
    }
}
