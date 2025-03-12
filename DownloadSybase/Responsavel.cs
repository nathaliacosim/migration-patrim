using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class Responsavel
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Responsavel(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task<List<ResponsavelBethaDba>> BuscarResponsaveis()
    {
        const string query = "SELECT i_respons, nome, cpf, funcao FROM bethadba.responsaveis;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<ResponsavelBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<ResponsavelBethaDba>();
        }
    }

    public async Task InsertIntoResponsaveis()
    {
        var dados = await BuscarResponsaveis();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM responsaveis_cloud WHERE i_responsavel = @i_responsavel";
            const string insertQuery = @"INSERT INTO responsaveis_cloud (id_cloud, i_responsavel, nome, cpf, funcao, observacoes) 
                                         VALUES (@id_cloud, @i_responsavel, @nome, @cpf, @funcao, @observacoes)";

            var parametros = new
            {
                id_cloud = "",
                i_responsavel = item.i_respons,
                nome = item.nome.ToUpper(),
                cpf = item.cpf ?? "",
                funcao = item.funcao == null ? "" : item.funcao.ToUpper(),
                observacoes = item.cpf == "" ? "CPF gerado em 4DEVS" : ""
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
                Console.WriteLine($"Erro ao inserir responsaveis_cloud: {ex.Message}");
            }
        }
    }
}
