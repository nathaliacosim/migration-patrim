using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarFornecedores
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarFornecedores(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/fornecedores");
    }

    public async Task<List<Models.ModelPostgres.Fornecedor>> BuscarFornecedores()
    {
        const string query = "SELECT * FROM public.fornecedores_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.Fornecedor>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar fornecedores_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Fornecedor>();
        }
    }

    public async Task<List<string>> EnviarFornecedoresCloud()
    {
        var pessoas = await BuscarFornecedores();
        var lotesIds = new List<string>();

        foreach (var item in pessoas)
        {
            var json = JsonConvert.SerializeObject(new FornecedorPOST
            {
                nome = item.nome,
                cpfCnpj = item.cpf_cnpj,
                tipo = new TipoFornecedorPOST
                {
                    valor = item.tipo == "J" ? "JURIDICA" : "FISICA",
                    descricao = item.tipo == "J" ? "Jurídica" : "Física"
                },
                situacao = new SituacaoFornecedorPOST
                {
                    valor = "ATIVO",
                    descricao = "Ativo"
                }
            });


            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar fornecedores_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar fornecedores_cloud {item.id}: {ex.Message}\n");
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
        const string query = "UPDATE fornecedores_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para fornecedores_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para fornecedores_cloud {id}: {ex.Message}");
        }
    }
}
