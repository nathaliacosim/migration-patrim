using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarLogradouroFornecedores
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarLogradouroFornecedores(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/logradouros");

        Task t = EnviarLogradouros();
    }

    public async Task<List<Models.ModelPostgres.Logradouro>> BuscarLogradouros()
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            string query = @"SELECT * FROM public.enderecos_cloud";
            return (await connection.QueryAsync<Models.ModelPostgres.Logradouro>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela public.enderecos_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Logradouro>();
        }
    }

    public async Task<List<string>> EnviarLogradouros()
    {
        var listaEnviar = await BuscarLogradouros();
        var lotesIds = new List<string>();
        foreach (var item in listaEnviar)
        {
            var json = new LogradouroPOST
            {
               nome = item.logradouro_atual.Trim().ToUpper(),
               tipoLogradouro = new TipoLogradouroLogradouroPOST
               {
                   id = item.id_tipo_logradouro
               },
               municipio = new MunicipioLogradouroPOST
               {
                   id = item.id_cidade
               },
               cep = item.cep
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
                        Console.WriteLine($"Erro ao enviar enderecos_cloud: id = {item.id}\nErro: {erro["message"]}\n");
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
                Console.WriteLine($"Erro ao enviar enderecos_cloud: id = {item.id}\nErro: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: enderecos_cloud\n\n");
        return lotesIds;
    }

    public async Task AtualizarIdCloud(int id, string id_cloud)
    {
        const string updateQuery = @"UPDATE enderecos_cloud 
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
            Console.WriteLine($"enderecos_cloud {id} atualizado com id_cloud = {id_cloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar id_cloud: enderecos_cloud {id}: {ex.Message}");
        }
    }
}
