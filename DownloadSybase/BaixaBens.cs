using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

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
        const string query = @"SELECT i_baixa, i_motivo, i_bem, data_baixa, historico, i_entidades FROM bethadba.baixas;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<BaixaBensBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<BaixaBensBethaDba>();
        }
    }

    public async Task InsertIntoBaixaBens()
    {
        var dados = await BuscarBaixaBens();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM baixas_cloud WHERE i_baixa = @i_baixa";
            const string insertQuery = @"INSERT INTO baixas_cloud 
                                           (id_cloud, i_baixa, i_motivo, i_bem, id_cloud_baixa, data_baixa, nota_explicativa, i_entidades)
                                         VALUES 
                                            (@id_cloud, @i_baixa, @i_motivo, @i_bem, @id_cloud_baixa, @data_baixa, @nota_explicativa, @i_entidades)";

            var parametros = new
            {
                id_cloud = "",
                i_baixa = item.i_baixa,
                i_motivo = item.i_motivo,
                i_bem = item.i_bem,
                id_cloud_baixa = (int?)null,
                data_baixa = item.data_baixa,
                nota_explicativa = item.historico,
                i_entidades = item.i_entidades
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_baixa });

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
                Console.WriteLine($"Erro ao inserir baixas_cloud: {ex.Message}");
            }
        }
    }
}
