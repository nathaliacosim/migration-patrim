using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class TipoBaixa
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public TipoBaixa(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<MotivoBethaDba>> BuscarTipoBaixa()
    {
        const string query = @"SELECT * FROM bethadba.motivos;";

        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<MotivoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<MotivoBethaDba>();
        }
    }

    public async Task InsertIntoTipoBaixa()
    {
        var dados = await BuscarTipoBaixa();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM tipo_baixa_cloud WHERE i_motivo = @i_motivo";
            const string insertQuery = @"INSERT INTO tipo_baixa_cloud (id_cloud, i_motivo, descricao, classificacao, i_entidades)
                                         VALUES (@id_cloud, @i_motivo, @descricao, @classificacao, @i_entidades);";

            var parametros = new
            {
                id_cloud = "",
                item.i_motivo,
                descricao = item.descricao?.Trim() ?? null,
                classificacao = "OUTROS",
                item.i_entidades
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_motivo });

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
                Console.WriteLine($"Erro ao inserir tipo_baixa_cloud: {ex.Message}");
            }
        }
    }
}