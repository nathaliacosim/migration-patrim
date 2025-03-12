using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using migracao-patrim.Models.ModelCloud;
using Newtonsoft.Json;

namespace migracao-patrim.Controller;

public class EnviarOrganogramasDepara
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarOrganogramasDepara(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/organogramas");
    }

    public async Task<List<Models.ModelPostgres.OrganogramaDePara>> BuscarOrganogramas()
    {
        const string query = "SELECT * FROM public.organograma_depara";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.OrganogramaDePara>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar organogramas: {ex.Message}");
            return new List<Models.ModelPostgres.OrganogramaDePara>();
        }
    }

    public async Task<List<string>> EnviarOrganogramasCloud()
    {
        var organogramas = await BuscarOrganogramas();
        var lotesIds = new List<string>();
        int configIdEmUso = 17351;

        foreach (var item in organogramas)
        {
            var nivelOrganograma = 0;
            if (item.unidade_numero == "000" && item.centro_custo_numero == "00000")
            {
                nivelOrganograma = 1;
            }
            else if (item.centro_custo_numero == "00000" && item.unidade_numero != "000")
            {
                nivelOrganograma = 2;
            } else
            {
                nivelOrganograma = 3;
            }

            var json = JsonConvert.SerializeObject(new OrganogramaPOST
            {
                configuracaoOrganograma = new ConfiguracaoOrganogramaOrganogramaPOST { id = configIdEmUso },
                numeroOrganograma = "0"+item.numero_organograma,
                descricao = item.descricao_organograma,
                nivel = nivelOrganograma
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar organograma {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar organograma {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("Processo de envio finalizado.\n\n");
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
        const string query = "UPDATE organograma_depara SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para organograma {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para organograma {id}: {ex.Message}");
        }
    }
}