using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class Enderecos
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Enderecos(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task<List<EnderecoBethaDba>> BuscarEnderecos()
    {
        const string query = "SELECT DISTINCT i_fornec, endereco, numero, bairro, cep, cidade, uf FROM bethadba.fornecedores WHERE endereco is not null ORDER BY endereco, numero;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<EnderecoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<EnderecoBethaDba>();
        }
    }

    public async Task InsertIntoEndereco()
    {
        var dados = await BuscarEnderecos();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM enderecos_cloud WHERE logradouro = @logradouro and numero = @numero and cidade = @cidade and estado = @estado";
            const string insertQuery = @"INSERT INTO enderecos_cloud 
                                               (id_cloud, i_fornec, logradouro, numero, bairro, cidade, estado, cep) 
                                         VALUES 
                                               (@id_cloud, @i_fornec, @logradouro, @numero, @bairro, @cidade, @estado, @cep)";

            var parametros = new
            {
                id_cloud = "",
                i_fornec = item.i_fornec,
                logradouro = item.endereco,
                item.numero,
                item.cidade,
                item.bairro,
                estado = item.uf,
                item.cep
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
                Console.WriteLine($"Erro ao inserir enderecos_cloud: {ex.Message}");
            }
        }
    }
}
