using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarEspecieBem
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarEspecieBem(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/especies-bem");
    }

    public async Task<List<Models.ModelPostgres.EspecieBem>> BuscarEspecieBem()
    {
        const string query = "SELECT * FROM public.especie_bem_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.EspecieBem>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar especie_bem_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.EspecieBem>();
        }
    }

    public async Task<List<string>> EnviarEspecieBemCloud()
    {
        var tipos = await BuscarEspecieBem();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new EspecieBemPOST
            {
                descricao = item.descricao.ToUpper(),
                grupoBem = new GrupoBemEspecieBemPOST
                {
                    id = item.id_grupo_bem
                }
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar especie_bem_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar especie_bem_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: especie_bem_cloud.\n\n");
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
        const string query = "UPDATE especie_bem_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para especie_bem_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para especie_bem_cloud {id}: {ex.Message}");
        }
    }
}
