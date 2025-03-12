using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

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
        const string query = @"SELECT i_baixa, CONVERT(VARCHAR(10), data_baixa, 120) as dt_baixa, i_bem, i_motivo, historico AS observacao FROM bethadba.baixas";
        try
        {
            Console.WriteLine("🔍 Buscando cabecalhos de baixas no banco Sybase...");
            using var connection = _odbcConnection.GetConnection();
            var result = (await connection.QueryAsync<BaixaCabecalhoBethaDba>(query)).AsList();

            Console.WriteLine($"✅ {result.Count} registros encontrados.");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar os dados: {ex.Message}");
            return new List<BaixaCabecalhoBethaDba>();
        }
    }

    public async Task InsertIntoCabecalhoBaixa()
    {
        Console.WriteLine("💾 Iniciando processo de inserção dos cabecalhos de baixas...");
        var dados = await BuscarCabecalhoBaixa();

        if (dados.Count == 0)
        {
            Console.WriteLine("⚠️ Nenhum dado encontrado para inserção.");
            return;
        }

        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM baixa_cabecalho_cloud WHERE i_baixa = @i_baixa";
            const string insertQuery = @"INSERT INTO baixa_cabecalho_cloud 
                                           (id_cloud, mes, ano, mes_ano, observacao, dt_baixa, i_baixa, i_bem, i_motivo, id_cloud_tipo_baixa, finalizado, id_cloud_finalizacao)
                                         VALUES 
                                            (@id_cloud, @mes, @ano, @mes_ano, @observacao, @dt_baixa, @i_baixa, @i_bem, @i_motivo, @id_cloud_tipo_baixa, @finalizado, @id_cloud_finalizacao)";

            var dataBaixa = item.dt_baixa.Split('-');
            var ano = dataBaixa[0];
            var mes = dataBaixa[1].PadLeft(2, '0');
            var mes_ano = mes + ano;

            var parametros = new
            {
                id_cloud = "",
                item.i_baixa,
                item.i_bem,
                ano,
                mes,
                mes_ano,
                observacao = item.observacao?.Trim() ?? null,
                item.dt_baixa,
                item.i_motivo,
                id_cloud_tipo_baixa = (int?)null,
                finalizado = 'N',
                id_cloud_finalizacao = (string)null
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_baixa });

                if (count == 0)
                {
                    _pgConnection.Execute(insertQuery, parametros);
                    Console.WriteLine($"✅ Registro {item.i_baixa} inserido com sucesso! 🎉");
                }
                else
                {
                    Console.WriteLine($"⚠️ Registro {item.i_baixa} já existe no banco.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao inserir baixa_cabecalho_cloud (ID {item.i_baixa}): {ex.Message}");
            }
        }

        Console.WriteLine("🚀 Processo de inserção concluído!");
    }
}
