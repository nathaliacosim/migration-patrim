using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class EspecieBem
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public EspecieBem(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _pgConnection = pgConnect;
    }

    private async Task<List<T>> BuscarDados<T>(string query)
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<T>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar dados: {ex.Message}");
            return new List<T>();
        }
    }

    public Task<List<DetalhamentoBethaDba>> BuscarDetalhamentos() =>
        BuscarDados<DetalhamentoBethaDba>(
            @"SELECT DISTINCT b.i_conta as i_conta, d.descricao as descricao, d.i_detalhamentos_bens as i_chave, 'D' as tipo_chave 
              FROM detalhamentos_bens_cloud d, bens_cloud b 
              WHERE d.i_entidades = b.i_entidades AND d.i_detalhamentos_bens = b.i_detalhamentos_bens 
              ORDER BY descricao;"
        );

    public Task<List<TipoNaturezaBethaDba>> BuscarTipoNaturezas() =>
        BuscarDados<TipoNaturezaBethaDba>(
            @"SELECT DISTINCT b.i_conta as i_conta, tn.i_tipo_natur as i_chave, tn.nome as descricao, 'N' as tipo_chave
              FROM bens_cloud b, tipos_natureza_bens_cloud tn 
              WHERE b.i_entidades = tn.i_entidades AND b.i_tipo_natur = tn.i_tipo_natur
              ORDER BY descricao;"
        );

    public async Task<List<int>> BuscarGrupoBens(int i_conta)
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<int>(
                "SELECT id_cloud FROM public.grupo_bem_cloud WHERE i_conta = @i_conta;",
                new { i_conta })
            ).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar grupo_bem_cloud: {ex.Message}");
            return new List<int>();
        }
    }

    private async Task InserirDados<T>(List<T> dados, int id_grupo)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM especie_bem_cloud WHERE descricao = @descricao AND id_grupo_bem = @id_grupo_bem;";
        const string insertQuery = "INSERT INTO especie_bem_cloud (id_cloud, id_grupo_bem, i_conta, i_chave, tipo_chave, descricao) " +
                                   "VALUES (@id_cloud, @id_grupo_bem, @i_conta, @i_chave, @tipo_chave, @descricao);";

        using var connection = _pgConnection.GetConnection();

        foreach (var item in dados)
        {
            try
            {
                var parametros = new
                {
                    id_cloud = "",
                    id_grupo_bem = id_grupo,
                    i_conta = item.GetType().GetProperty("i_conta")?.GetValue(item, null),
                    i_chave = item.GetType().GetProperty("i_chave")?.GetValue(item, null),
                    tipo_chave = item.GetType().GetProperty("tipo_chave")?.GetValue(item, null)?.ToString(),
                    descricao = item.GetType().GetProperty("descricao")?.GetValue(item, null)?.ToString()
                };

                int count = await connection.ExecuteScalarAsync<int>(checkExistsQuery, parametros);

                if (count == 0)
                {
                    await connection.ExecuteAsync(insertQuery, parametros);
                    Console.WriteLine("Registro inserido com sucesso.");
                }
                else
                {
                    Console.WriteLine("Registro já existente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir dados: {ex.Message}");
            }
        }
    }

    public async Task InsertIntoEspecies()
    {
        var detalhamentos = await BuscarDetalhamentos();
        var tipoNaturezas = await BuscarTipoNaturezas();

        using var connection = _pgConnection.GetConnection();

        foreach (var detalhamento in detalhamentos)
        {
            var grupoBens = await BuscarGrupoBens(detalhamento.i_conta);
            foreach (var grupo in grupoBens)
            {
                await InserirDados(detalhamentos, grupo);
            }
        }

        foreach (var tipoNatureza in tipoNaturezas)
        {
            var grupoBens = await BuscarGrupoBens(tipoNatureza.i_conta);
            foreach (var grupo in grupoBens)
            {
                await InserirDados(tipoNaturezas, grupo);
            }
        }
    }
}
