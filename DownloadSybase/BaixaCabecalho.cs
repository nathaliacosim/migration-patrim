using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class BaixaCabecalho
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public BaixaCabecalho(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<BaixaCabecalhoBethaDba>> BuscarCabecalhoBaixa()
    {
        const string query = @"SELECT 
                                    CONVERT(VARCHAR(10), data_baixa, 120) as dt_baixa,
                                    i_motivo,
                                    historico AS observacao
                               FROM bethadba.baixas
                               GROUP BY 
                                    CONVERT(VARCHAR(10), data_baixa, 120),
                                    i_motivo,
                                    historico
                               ORDER BY dt_baixa, i_motivo;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<BaixaCabecalhoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<BaixaCabecalhoBethaDba>();
        }
    }

    public async Task InsertIntoCabecalhoBaixa()
    {
        var dados = await BuscarCabecalhoBaixa();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM baixa_cabecalho_cloud WHERE i_motivo = @i_motivo and dt_baixa = @dt_baixa";
            const string insertQuery = @"INSERT INTO baixa_cabecalho_cloud 
                                           (id_cloud, mes, ano, mes_ano, observacao, dt_baixa, i_motivo, id_cloud_tipo_baixa)
                                         VALUES 
                                            (@id_cloud, @mes, @ano, @mes_ano, @observacao, @dt_baixa, @i_motivo, @id_cloud_tipo_baixa)";

            var dataBaixa = item.dt_baixa.Split('-');
            var ano = dataBaixa[0];
            var mes = dataBaixa[1].PadLeft(2, '0');
            var mes_ano = mes + ano;

            var parametros = new
            {
                id_cloud = "",
                ano = ano,
                mes = mes,
                mes_ano = mes_ano,
                observacao = item.observacao?.Trim() ?? "",
                dt_baixa = item.dt_baixa,
                i_motivo = item.i_motivo,
                id_cloud_tipo_baixa = (int?)null
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_motivo, item.dt_baixa });

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
                Console.WriteLine($"Erro ao inserir baixa_cabecalho_cloud: {ex.Message}");
            }
        }
    }
}
