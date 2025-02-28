using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class TransferenciaCabecalho
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public TransferenciaCabecalho(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<TransferenciaCabecalhoBethaDba>> BuscarCabecalhoTransf()
    {
        const string query = @"SELECT 
                                    CONVERT(VARCHAR(10), data_transf, 120) AS data_transferencia,
                                    tipo AS tipo_transferencia,
                                    historico AS observacao,
                                    novo_cod1 AS novo1,
                                    novo_cod2 AS novo2
                               FROM bethadba.localizacoes
                               GROUP BY 
                                    CONVERT(VARCHAR(10), data_transf, 120),
                                    tipo,
                                    historico,
                                    novo_cod1,
                                    novo_cod2
                               ORDER BY data_transferencia;";

        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<TransferenciaCabecalhoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<TransferenciaCabecalhoBethaDba>();
        }
    }

    public async Task InsertIntoCabecalhoTransf()
    {
        var dados = await BuscarCabecalhoTransf();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM transferencia_cabecalho_cloud WHERE data_transferencia = @data_transferencia and tipo_transferencia = @tipo_transferencia and novo1 = @novo1 and (novo2 = null OR novo2 = @novo2)";
            const string insertQuery = @"INSERT INTO transferencia_cabecalho_cloud 
                                           (id_cloud, mes, ano, mes_ano, observacao, data_transferencia, tipo_transferencia, novo1, novo2)
                                         VALUES 
                                            (@id_cloud, @mes, @ano, @mes_ano, @observacao, @data_transferencia, @tipo_transferencia, @novo1, @novo2)";

            var dataTransf = item.data_transferencia.Split('-');
            var ano = dataTransf[0];
            var mes = dataTransf[1].PadLeft(2, '0');
            var mes_ano = mes + ano;

            var parametros = new
            {
                id_cloud = "",
                ano = ano,
                mes = mes,
                mes_ano = mes_ano,
                observacao = item.observacao?.Trim() ?? "",
                data_transferencia = item.data_transferencia,
                item.tipo_transferencia,
                item.novo1,
                item.novo2
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.data_transferencia, item.tipo_transferencia, item.novo1, item.novo2 });

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
                Console.WriteLine($"Erro ao inserir transferencia_cabecalho_cloud: {ex.Message}");
            }
        }
    }
}
