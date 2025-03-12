using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class CentroCusto
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public CentroCusto(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task<List<CustoBethaDba>> BuscarCentroCusto()
    {
        const string query = "SELECT i_custo, i_unidade, i_unid_orc, nome, i_respons, ativo FROM bethadba.custos";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<CustoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<CustoBethaDba>();
        }
    }

    public async Task InsertIntoCentroCusto()
    {
        var dados = await BuscarCentroCusto();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM centro_custos_cloud WHERE i_custo = @i_custo and i_unidade = @i_unidade and i_unid_orc = @i_unid_orc;";
            const string insertQuery = @"INSERT INTO centro_custos_cloud (id_cloud, i_custo, i_unidade, i_unid_orc, nome, i_respons, ativo) 
                                         VALUES (@id_cloud, @i_custo, @i_unidade, @i_unid_orc, @nome, @i_respons, @ativo)";

            var parametros = new
            {
                id_cloud = "",
                item.i_custo,
                item.i_unidade,
                item.i_unid_orc,
                item.i_respons,
                item.nome,
                item.ativo
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
                Console.WriteLine($"Erro ao inserir centro_custos_cloud: {ex.Message}");
            }
        }
    }
}
