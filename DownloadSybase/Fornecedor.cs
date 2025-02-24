using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Dapper;

namespace MigraPatrim.DownloadSybase;

public class Fornecedor
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Fornecedor(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task<List<FornecedorBethaDba>> BuscarFornecedores()
    {
        const string query = "SELECT i_fornec, nome, endereco, numero, bairro, cep, cidade, uf, cgc, cpf, telefone, email, tipo FROM bethadba.fornecedores;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<FornecedorBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<FornecedorBethaDba>();
        }
    }

    public async Task InsertIntoFornecedor()
    {
        var dados = await BuscarFornecedores();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM fornecedores_cloud WHERE nome = @nome and cpf_cnpj = @cpf_cnpj";
            const string insertQuery = @"INSERT INTO fornecedores_cloud 
                                               (id_cloud, i_fornec, nome, cpf_cnpj, tipo, telefone, email,
                                                end_logradouro, end_numero, end_bairro, end_cep, end_cidade, end_uf) 
                                         VALUES 
                                               (@id_cloud, @i_fornec, @nome, @cpf_cnpj, @tipo, @telefone, @email,
                                                @end_logradouro, @end_numero, @end_bairro, @end_cep, @end_cidade, @end_uf)";

            var parametros = new
            {
                id_cloud = "",
                item.i_fornec,
                nome = item.nome.ToUpper(),
                cpf_cnpj = item.cpf == null ? item.cgc : item.cpf,
                item.tipo,
                item.telefone,
                item.email,
                end_logradouro = item.endereco,
                end_numero = item.numero,
                end_bairro = item.bairro,
                end_cep = item.cep,
                end_cidade = item.cidade,
                end_uf = item.uf
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, parametros);

                if (count == 0)
                {
                    _pgConnection.Execute(insertQuery, parametros);
                    Console.WriteLine("Registro inserido com sucesso.");
                }
                else
                {
                    Console.WriteLine("Registro já existente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir fornecedores_cloud: {ex.Message}");
            }
        }
    }
}
