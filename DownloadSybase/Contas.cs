using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class Contas
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Contas(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<ContasBethaDba>> BuscarContas()
    {
        const string query = "SELECT * FROM bethadba.contas;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<ContasBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<ContasBethaDba>();
        }
    }

    public async Task InsertIntoContas()
    {
        var dados = await BuscarContas();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM contas_cloud WHERE i_conta = @i_conta";
            const string insertQuery = @"INSERT INTO contas_cloud 
                                           (id_cloud, i_conta, descricao, tipo, tipo_bem, proc_aquisicao,
                                            classif, percent_deprec, vlr_resid, i_entidades, codigo_tce)
                                         VALUES 
                                            (@id_cloud, @i_conta, @descricao, @tipo, @tipo_bem, @proc_aquisicao,
                                             @classif, @percent_deprec, @vlr_resid, @i_entidades, @codigo_tce)";

            var parametros = new
            {
                id_cloud = "",
                item.i_conta,
                item.descricao,
                item.tipo,
                item.tipo_bem,
                item.proc_aquisicao,
                item.classif,
                item.percent_deprec,
                item.vlr_resid,
                item.i_entidades,
                item.codigo_tce
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_conta });

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
                Console.WriteLine($"Erro ao inserir contas_cloud: {ex.Message}");
            }
        }
    }
}
