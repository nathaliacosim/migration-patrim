using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class TipoNatureza
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public TipoNatureza(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<TiposNaturezaBethaDba>> BuscarTiposNatureza()
    {
        const string query = "SELECT * FROM bethadba.tipo_natur;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<TiposNaturezaBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<TiposNaturezaBethaDba>();
        }
    }

    public async Task InsertIntoTiposNatureza()
    {
        var dados = await BuscarTiposNatureza();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM tipos_natureza_bens_cloud WHERE i_tipo_natur = @i_tipo_natur";
            const string insertQuery = @"INSERT INTO tipos_natureza_bens_cloud 
                                           (i_tipo_natur, nome, i_entidades, veiculo)
                                         VALUES 
                                           (@i_tipo_natur, @nome, @i_entidades, @veiculo);";

            var parametros = new
            {
                i_tipo_natur = item.i_tipo_natur,
                nome = item.nome?.Trim() ?? "",
                i_entidades = item.i_entidades,
                veiculo = item.veiculo?.Trim() ?? ""
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_tipo_natur });

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
                Console.WriteLine($"Erro ao inserir tipos_natureza_bens_cloud: {ex.Message}");
            }
        }
    }
}
