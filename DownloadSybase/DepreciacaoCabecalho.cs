using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class DepreciacaoCabecalho
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public DepreciacaoCabecalho(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<DepreciacaoCabecalhoBethaDba>> BuscarCabecalhoDepre()
    {
        const string query = @"SELECT DISTINCT
                                 RIGHT('0' + CAST(MONTH(data_depr) AS VARCHAR), 2) AS mes,
                                 CAST(YEAR(data_depr) AS VARCHAR) AS ano,
                                 RIGHT('0' + CAST(MONTH(data_depr) AS VARCHAR), 2) + CAST(YEAR(data_depr) AS VARCHAR) AS mes_ano
                               FROM bethadba.depreciacoes
                               ORDER BY ano, mes ASC;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<DepreciacaoCabecalhoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<DepreciacaoCabecalhoBethaDba>();
        }
    }

    public async Task InsertIntoCabecalhoDepre()
    {
        var dados = await BuscarCabecalhoDepre();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM depreciacao_cabecalho_cloud WHERE mes_ano = @mes_ano";
            const string insertQuery = @"INSERT INTO depreciacao_cabecalho_cloud 
                                           (id_cloud, mes, ano, mes_ano, observacao)
                                         VALUES 
                                            (@id_cloud, @mes, @ano, @mes_ano, @observacao)";

            var parametros = new
            {
                id_cloud = "",
                item.mes,
                item.ano,
                item.mes_ano,
                observacao = ""
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.mes_ano });

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
                Console.WriteLine($"Erro ao inserir depreciacao_cabecalho_cloud: {ex.Message}");
            }
        }
    }
}
