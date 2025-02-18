using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarExercicios
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarExercicios(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/parametros-exercicios");

        Task t = EnviarParametrosExercicios();
    }

    public async Task<List<Models.ModelPostgres.Exercicios>> BuscarExercicios()
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            string query = @"SELECT * FROM public.exercicio_cloud";
            return (await connection.QueryAsync<Models.ModelPostgres.Exercicios>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela public.exercicio_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Exercicios>();
        }
    }

    public async Task<List<string>> EnviarParametrosExercicios()
    {
        var listaEnviar = await BuscarExercicios();
        var lotesIds = new List<string>();
        foreach (var item in listaEnviar)
        {
            var json = new ExercicioPOST
            {
                exercicio = item.exercicio,
                configuracaoOrganograma = new ConfiguracaoOrganogramaExercicioPOST
                {
                    id = item.id_configuracao
                }
            };

            var dadosJson = JsonConvert.SerializeObject(json);
            Console.WriteLine($"Enviando dados: {dadosJson}\n");

            try
            {
                var resposta = await _postRequest.Send(dadosJson).ConfigureAwait(false);

                if (resposta.Contains("message"))
                {
                    try
                    {
                        var erro = JsonConvert.DeserializeObject<Dictionary<string, string>>(resposta);
                        Console.WriteLine($"Erro ao enviar exercicio_cloud: id = {item.id}\nErro: {erro["message"]}\n");
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"Erro ao tentar deserializar a resposta de erro: {jsonEx.Message}\n");
                        Console.WriteLine($"Resposta original: {resposta}\n");
                    }
                }
                else
                {
                    var loteId = resposta;
                    lotesIds.Add(loteId);
                    Console.WriteLine("AtualizarIdCloud(item.id, loteId): " + item.id + " " + loteId);
                    await AtualizarIdCloud(item.id, loteId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar exercicio_cloud: id = {item.id}\nErro: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: exercicio_cloud\n\n");
        return lotesIds;
    }

    public async Task AtualizarIdCloud(int id, string id_cloud)
    {
        const string updateQuery = @"UPDATE exercicio_cloud 
                                     SET id_cloud = @id_cloud
                                     WHERE id = @id";

        var parameters = new
        {
            id,
            id_cloud
        };

        try
        {
            await _pgConnection.ExecuteAsync(updateQuery, parameters);
            Console.WriteLine($"exercicio_cloud {id} atualizado com id_cloud = {id_cloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar id_cloud: exercicio_cloud {id}: {ex.Message}");
        }
    }
}
