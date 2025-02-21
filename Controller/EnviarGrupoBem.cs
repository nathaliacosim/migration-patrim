using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarGrupoBem
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarGrupoBem(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/grupos-bem");
    }

    public async Task<List<Models.ModelPostgres.GrupoBem>> BuscarGrupoBem()
    {
        const string query = "SELECT * FROM public.grupo_bem_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.GrupoBem>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar grupo_bem_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.GrupoBem>();
        }
    }

    public async Task<List<string>> EnviarGrupoBemCloud()
    {
        var tipos = await BuscarGrupoBem();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new GrupoBemPOST
            {
                descricao = item.descricao,
                metodoDepreciacao = new MetodoDepreciacaoGrupoBemPOST
                {
                    id = item.id_metodo_depreciacao
                },
                tipoBem = new TipoBemGrupoBemPOST {
                    id = item.id_tipo_bem
                },
                percentualDepreciacaoAnual = item.percentual_depreciacao,
                percentualValorResidual = item.percentual_residual,
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar grupo_bem_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar grupo_bem_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: grupo_bem_cloud.\n\n");
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
        const string query = "UPDATE grupo_bem_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para grupo_bem_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para grupo_bem_cloud {id}: {ex.Message}");
        }
    }
}
