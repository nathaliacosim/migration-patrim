using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class BaixaEstornos
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public BaixaEstornos(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<BaixaEstornoBethaDba>> BuscarEstornoBaixaBens()
    {
        const string query = @"SELECT * FROM bethadba.estornos;";
        try
        {
            Console.WriteLine("🔍 Buscando estornos de baixas no banco Sybase...");
            using var connection = _odbcConnection.GetConnection();
            var result = (await connection.QueryAsync<BaixaEstornoBethaDba>(query)).AsList();

            Console.WriteLine($"✅ {result.Count} registros encontrados.");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar os dados: {ex.Message}");
            return new List<BaixaEstornoBethaDba>();
        }
    }

    public async Task InsertIntoEstornoBaixaBens()
    {
        Console.WriteLine("💾 Iniciando processo de inserção dos estornos de baixas...");
        var dados = await BuscarEstornoBaixaBens();

        if (dados.Count == 0)
        {
            Console.WriteLine("⚠️ Nenhum dado encontrado para inserção.");
            return;
        }

        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM estornos_baixas_cloud WHERE i_estorno = @i_estorno";
            const string insertQuery = @"INSERT INTO estornos_baixas_cloud 
                                           (id_cloud, i_estorno, i_baixa, i_motivo, i_bem, id_cloud_bem, id_cloud_baixa, data_estorno, nota_explicativa, i_entidades)
                                         VALUES 
                                            (@id_cloud, @i_estorno, @i_baixa, @i_motivo, @i_bem, @id_cloud_bem, @id_cloud_baixa, @data_estorno, @nota_explicativa, @i_entidades)";

            var parametros = new
            {
                id_cloud = "",
                item.i_estorno,
                item.i_baixa,
                item.i_motivo,
                item.i_bem,
                id_cloud_bem = (int?)null,
                id_cloud_baixa = (int?)null,
                data_estorno = item.data_est,
                nota_explicativa = item.historico?.Trim() ?? null,
                item.i_entidades
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_estorno });

                if (count == 0)
                {
                    _pgConnection.Execute(insertQuery, parametros);
                    Console.WriteLine($"✅ Registro {item.i_estorno} inserido com sucesso! 🎉");
                }
                else
                {
                    Console.WriteLine($"⚠️ Registro {item.i_estorno} já existe no banco.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao inserir estornos_baixas_cloud (ID {item.i_estorno}): {ex.Message}");
            }
        }

        Console.WriteLine("🚀 Processo de inserção concluído!");
    }
}
