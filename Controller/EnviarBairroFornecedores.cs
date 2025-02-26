using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarBairroFornecedores
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarBairroFornecedores(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/bairros");

        Task t = EnviarBairros();
    }

    public async Task<List<Models.ModelPostgres.Bairros>> BuscarBairros()
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            string query = @"SELECT * FROM public.bairros_cloud";
            return (await connection.QueryAsync<Models.ModelPostgres.Bairros>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela public.bairros_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Bairros>();
        }
    }

    public async Task<List<string>> EnviarBairros()
    {
        var listaEnviar = await BuscarBairros();
        var lotesIds = new List<string>();
        foreach (var item in listaEnviar)
        {
            var json = new BairroPOST
            {
                nome = item.nome.ToUpper(),
                municipio = new MunicipioBairroPOST { id = item.id_municipio }
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
                        Console.WriteLine($"Erro ao enviar bairros_cloud: id = {item.id}\nErro: {erro["message"]}\n");
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
                Console.WriteLine($"Erro ao enviar bairros_cloud: id = {item.id}\nErro: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: bairros_cloud\n\n");
        return lotesIds;
    }

    public async Task AtualizarIdCloud(int id, string id_cloud)
    {
        const string updateQuery = @"UPDATE bairros_cloud 
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
            Console.WriteLine($"bairros_cloud {id} atualizado com id_cloud = {id_cloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar id_cloud: bairros_cloud {id}: {ex.Message}");
        }
    }
}
