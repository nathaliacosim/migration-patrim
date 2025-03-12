using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class DepreciacaoBens
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public DepreciacaoBens(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<DepreciacaoBensBethaDba>> BuscarBensDepreciacao()
    {
        const string query = @"SELECT i_depreciacao, i_bem, data_depr, valor_calc, i_entidades FROM bethadba.depreciacoes;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<DepreciacaoBensBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<DepreciacaoBensBethaDba>();
        }
    }

    public async Task InsertIntoBensDepreciacao()
    {
        var dados = await BuscarBensDepreciacao();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM depreciacoes_cloud WHERE i_depreciacao = @i_depreciacao";
            const string insertQuery = @"INSERT INTO depreciacoes_cloud 
                                           (id_cloud, i_depreciacao, i_bem, id_cloud_depreciacao, data_depreciacao, valor_depreciado, i_entidades)
                                         VALUES 
                                           (@id_cloud, @i_depreciacao, @i_bem, @id_cloud_depreciacao, @data_depreciacao, @valor_depreciado, @i_entidades)";

            var parametros = new
            {
                id_cloud = "",
                item.i_depreciacao,
                item.i_bem,
                id_cloud_depreciacao = (int?)null,
                data_depreciacao = item.data_depr,
                valor_depreciado = item.valor_calc,
                item.i_entidades
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_depreciacao });

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
                Console.WriteLine($"Erro ao inserir depreciacoes_cloud: {ex.Message}");
            }
        }
    }
}
