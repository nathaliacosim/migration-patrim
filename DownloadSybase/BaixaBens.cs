using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class BaixaBens
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public BaixaBens(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<BaixaBensBethaDba>> BuscarBaixaBens()
    {
        const string query = @"SELECT i_baixa, i_motivo, i_bem, CONVERT(VARCHAR(10), data_baixa, 120) as data_baixa, i_entidades FROM bethadba.baixas;";
        try
        {
            Console.WriteLine("🔍 Buscando baixas de bens no banco Sybase...");
            using var connection = _odbcConnection.GetConnection();
            var result = (await connection.QueryAsync<BaixaBensBethaDba>(query)).AsList();

            Console.WriteLine($"✅ {result.Count} registros encontrados.");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar os dados: {ex.Message}");
            return new List<BaixaBensBethaDba>();
        }
    }

    public async Task InsertIntoBaixaBens()
    {
        Console.WriteLine("💾 Iniciando processo de inserção das baixas de bens...");
        var dados = await BuscarBaixaBens();

        if (dados.Count == 0)
        {
            Console.WriteLine("⚠️ Nenhum dado encontrado para inserção.");
            return;
        }

        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM baixas_cloud WHERE i_baixa = @i_baixa";
            const string insertQuery = @"INSERT INTO baixas_cloud 
                                           (id_cloud, i_baixa, i_motivo, i_bem, id_cloud_bem, id_cloud_baixa, data_baixa, i_entidades)
                                         VALUES 
                                            (@id_cloud, @i_baixa, @i_motivo, @i_bem, @id_cloud_bem, @id_cloud_baixa, @data_baixa, @i_entidades)";

            var parametros = new
            {
                id_cloud = "",
                item.i_baixa,
                item.i_motivo,
                item.i_bem,
                id_cloud_bem = (int?)null,
                id_cloud_baixa = (int?)null,
                item.data_baixa,
                item.i_entidades
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
                Console.WriteLine($"❌ Erro ao inserir baixas_cloud (ID {item.i_baixa}): {ex.Message}");
            }
        }

        Console.WriteLine("🚀 Processo de inserção concluído!");
    }
}
