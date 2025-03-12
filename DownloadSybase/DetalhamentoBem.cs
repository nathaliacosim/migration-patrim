using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class DetalhamentoBem
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public DetalhamentoBem(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<DetalhamentosBethaDba>> BuscarDetalhamentos()
    {
        const string query = "SELECT * FROM bethadba.detalhamentos_bens;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<DetalhamentosBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<DetalhamentosBethaDba>();
        }
    }

    public async Task InsertIntoDetalhamentos()
    {
        var dados = await BuscarDetalhamentos();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM detalhamentos_bens_cloud WHERE i_detalhamentos_bens = @i_detalhamentos_bens";
            const string insertQuery = @"INSERT INTO detalhamentos_bens_cloud 
                                           (i_detalhamentos_bens, descricao, codigo_tce, categoria, natureza, padrao, utilizacao, i_entidades)
                                         VALUES 
                                           (@i_detalhamentos_bens, @descricao, @codigo_tce, @categoria, @natureza, @padrao, @utilizacao, @i_entidades);";

            var parametros = new
            {
                i_detalhamentos_bens = item.i_detalhamentos_bens,
                descricao = item.descricao?.Trim() ?? "",
                codigo_tce = item.codigo_tce?.ToString() ?? "",
                categoria = item.categoria?.ToString() ?? "",
                natureza = item.natureza?.ToString() ?? "",
                padrao = item.padrao?.ToString() ?? "",
                utilizacao = item.utilizacao?.ToString() ?? "",
                i_entidades = item.i_entidades
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_detalhamentos_bens });

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
                Console.WriteLine($"Erro ao inserir detalhamentos_bens_cloud: {ex.Message}");
            }
        }
    }
}
