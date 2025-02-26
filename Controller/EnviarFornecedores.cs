using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Models.ModelPostgres;
using MigraPatrim.Request;
using MigraPatrim.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

    public async Task<List<Fornecedor>> BuscarFornecedores()
    {
        const string query = "SELECT * FROM public.fornecedores_cloud WHERE id_cloud = ''";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Fornecedor>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar fornecedores_cloud: {ex.Message}");
            return new List<Fornecedor>();
        }
    }

    public Logradouro getLogFornecedor(int i_fornec)
    {
        const string query = "SELECT * FROM public.enderecos_cloud WHERE i_fornec = @i_fornec";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return connection.QueryFirstOrDefault<Logradouro>(query, new { i_fornec });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar enderecos_cloud: {ex.Message}");
            return null;
        }
    }

    public async Task<List<string>> EnviarFornecedoresCloud()
    {
        var pessoas = await BuscarFornecedores();
        var lotesIds = new List<string>();

        foreach (var item in pessoas)
        {
            var dadosEndereco = getLogFornecedor(item.i_fornec);

            var json = JsonConvert.SerializeObject(new FornecedorPOST
            {
                nome = item.nome.Trim().ToUpper(),
                cpfCnpj = item.cpf_cnpj,
                dataInclusao = "2025-01-01",
                tipo = new TipoFornecedorPOST
                {
                    valor = item.tipo == "J" ? "JURIDICA" : "FISICA",
                    descricao = item.tipo == "J" ? "Jurídica" : "Física"
                },
                situacao = new SituacaoFornecedorPOST
                {
                    valor = "ATIVO",
                    descricao = "Ativo"
                },
                endereco = item.end_logradouro != null ? new EnderecoFornecedorPOST
                {
                    descricao = "Endereço Principal",
                    numero = dadosEndereco.numero,
                    cep = dadosEndereco.cep,
                    municipio = new MunicipioFornecedorPOST
                    {
                        id = dadosEndereco.id_cidade
                    },
                    bairro = new BairroFornecedorPOST
                    {
                        id = dadosEndereco.id_bairro
                    },
                    logradouro = new LogradouroFornecedorPOST
                    {
                        id = int.Parse(dadosEndereco.id_cloud)
                    }
                } : null,
                telefones = item.telefone != null ? new List<TelefoneFornecedorPOST> 
                {
                    new TelefoneFornecedorPOST
                    {
                        numero = StringHelper.LimparTelefone(item.telefone),
                        tipo = "CELULAR",
                        observacao = "Telefone principal",
                        ordem = 0
                    }
                } : null,
                emails = item.email != null ? new List<EmailFornecedorPOST>
                {
                    new EmailFornecedorPOST
                    {
                        endereco = StringHelper.LimparEmail(item.email),
                        descricao = "Email principal",
                        ordem = 0
                    }
                } : null
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

        Console.WriteLine("FINALIZADO: fornecedores_cloud.\n\n");
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
