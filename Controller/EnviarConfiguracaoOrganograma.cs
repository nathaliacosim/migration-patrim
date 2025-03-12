using Dapper;
using System;
using Newtonsoft.Json;
using migracao-patrim.Request;
using System.Threading.Tasks;
using migracao-patrim.Connections;
using System.Collections.Generic;
using migracao-patrim.Models.ModelCloud;

namespace migracao-patrim.Controller;

public class EnviarConfiguracaoOrganograma
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarConfiguracaoOrganograma(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/configuracoes-organogramas");

        Task t = EnviarConfiguracoes();
    }

    public async Task<List<Models.ModelPostgres.ConfiguracaoOrganograma>> BuscarConfiguracoes()
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            string query = @"SELECT * FROM public.configuracao_organograma_cloud";
            return (await connection.QueryAsync<Models.ModelPostgres.ConfiguracaoOrganograma>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela public.configuracao_organograma_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.ConfiguracaoOrganograma>();
        }
    }

    public async Task<List<string>> EnviarConfiguracoes()
    {
        var listaEnviar = await BuscarConfiguracoes();
        var lotesIds = new List<string>();
        foreach (var item in listaEnviar)
        {
            var json = new ConfigOrganogramaPOST
            {
                descricao = item.descricao,
                mascara = item.mascara,
                niveis = new List<NiveisConfigOrganogramaPOST>
                {
                    new NiveisConfigOrganogramaPOST
                    {
                        nivel = 1,
                        descricao = "Orgão",
                        digitos = 2
                    },
                    new NiveisConfigOrganogramaPOST
                    {
                        nivel = 2,
                        descricao = "Unidade",
                        digitos = 3,
                        separador = "."
                    },
                    new NiveisConfigOrganogramaPOST
                    {
                        nivel = 3,
                        descricao = "Centro de Custo",
                        digitos = 5,
                        separador = "."
                    }
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
                        Console.WriteLine($"Erro ao enviar configuracao_organograma_cloud: id = {item.id}\nErro: {erro["message"]}\n");
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
                Console.WriteLine($"Erro ao enviar configuracao_organograma_cloud: id = {item.id}\nErro: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: configuracao_organograma_cloud\n\n");
        return lotesIds;
    }

    public async Task AtualizarIdCloud(int id, string id_cloud)
    {
        const string updateQuery = @"UPDATE configuracao_organograma_cloud 
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
            Console.WriteLine($"configuracao_organograma_cloud {id} atualizado com id_cloud = {id_cloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar id_cloud: configuracao_organograma_cloud {id}: {ex.Message}");
        }
    }
}
