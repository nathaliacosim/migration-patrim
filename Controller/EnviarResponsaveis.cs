using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarResponsaveis
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarResponsaveis(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/responsaveis");
    }

    public async Task<List<Models.ModelPostgres.Responsavel>> BuscarResponsaveis()
    {
        const string query = "SELECT * FROM public.responsaveis_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.Responsavel>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar responsaveis_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Responsavel>();
        }
    }

    public async Task<List<string>> EnviarResponsaveisCloud()
    {
        var tipos = await BuscarResponsaveis();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new ResponsavelPOST
            {
                nome = item.nome,
                cpf = item.cpf,
                funcao = item.funcao
            });
            

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar responsaveis_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar responsaveis_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: responsaveis_cloud.\n\n");
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
        const string query = "UPDATE responsaveis_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para responsaveis_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para responsaveis_cloud {id}: {ex.Message}");
        }
    }
}
