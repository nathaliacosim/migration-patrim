using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelPostgres;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class Organogramas
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Organogramas(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    private async Task<List<T>> BuscarDados<T>(string query)
    {
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<T>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar dados: {ex.Message}");
            return new List<T>();
        }
    }

    public Task<List<OrgaoBethaDba>> BuscarOrgaos() => BuscarDados<OrgaoBethaDba>("SELECT i_unidade as orgao, nome as descricao, 1 as nivel FROM bethadba.unidades;");
    public Task<List<UnidadeBethaDba>> BuscarUnidades() => BuscarDados<UnidadeBethaDba>("SELECT i_unidade as orgao, i_unid_orc as unidade, descricao, 2 as nivel FROM bethadba.unidades_orc;");
    public Task<List<CentroCustoBethaDba>> BuscarCentroCusto() => BuscarDados<CentroCustoBethaDba>("SELECT i_unidade as orgao, i_unid_orc as unidade, i_custo as centro_custo, nome as descricao, 3 as nivel FROM bethadba.custos;");

    public async Task<List<int>> BuscarConfiguracoesOrganograma()
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<int>("SELECT id_cloud FROM public.configuracao_organograma_cloud;")).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar configuracoes: {ex.Message}");
            return new List<int>();
        }
    }

    private async Task InserirDados<T>(List<T> dados, Func<T, string> numeroFunc, int idConfig, int nivel)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM organograma_cloud WHERE numero = @numero and id_configuracao = @id_configuracao";
        const string insertQuery = "INSERT INTO organograma_cloud (id_cloud, id_configuracao, descricao, numero, nivel) VALUES (@id_cloud, @id_configuracao, @descricao, @numero, @nivel)";

        using var connection = _pgConnection.GetConnection();

        foreach (var item in dados)
        {
            var parametros = new
            {
                id_cloud = "",
                id_configuracao = idConfig,
                descricao = item.GetType().GetProperty("descricao")?.GetValue(item, null)?.ToString()?.ToUpper(),
                numero = numeroFunc(item),
                nivel
            };

            try
            {
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

    public async Task InsertIntoOrganogramas()
    {
        var configuracoes = await BuscarConfiguracoesOrganograma();
        foreach (var config in configuracoes)
        {
            await InserirDados(await BuscarOrgaos(), o => o.orgao.PadLeft(2, '0') + "00000000", config, 1);
            await InserirDados(await BuscarUnidades(), u => u.orgao.PadLeft(2, '0') + u.unidade.PadLeft(3, '0') + "00000", config, 2);
            await InserirDados(await BuscarCentroCusto(), t => t.orgao.PadLeft(2, '0') + t.unidade.PadLeft(3, '0') + t.centro_custo.PadLeft(5, '0'), config, 3);
        }
    }
}
