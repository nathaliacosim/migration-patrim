using System;
using Dapper;
using System.Threading.Tasks;
using MigraPatrim.Connections;
using System.Collections.Generic;
using MigraPatrim.Models.ModelSybase;

namespace MigraPatrim.DownloadSybase;

public class ConfiguracaoOrganograma
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public ConfiguracaoOrganograma(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;

        Task task = MontarConfiguracoes();
    }

    public async Task<List<ExercicioBethaDba>> BuscarExercicios()
    {
        try
        {
            using var connection = _odbcConnection.GetConnection();
            string query = @"SELECT * FROM bethadba.exercicios";
            return (await connection.QueryAsync<ExercicioBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela bethadba.exercicios: {ex.Message}");
            return new List<ExercicioBethaDba>();
        }
    }

    public async Task MontarConfiguracoes()
    {
        var listaExercicios = await BuscarExercicios();
        foreach (var exercicio in listaExercicios)
        {
            Console.WriteLine($"Exercício: {exercicio.i_exercicio}");
            var configuracao = "Configuração organograma " + exercicio.i_exercicio;
            var mascara = "##.##.#####";
            var niveis = 3;

            InsertIntoConfiguracaoOrganograma(configuracao, mascara, niveis);
        }
    }

    public void InsertIntoConfiguracaoOrganograma(string configuracao, string mascara, int niveis)
    {
        const string checkExistsQuery = @"SELECT COUNT(1) FROM configuracao_organograma_cloud WHERE descricao = @descricao";
        const string insertQuery = @"INSERT INTO configuracao_organograma_cloud (id_cloud, descricao, mascara, niveis) VALUES (@id_cloud, @descricao, @mascara, @niveis)";
        var parametros = new { id_cloud = "", descricao = configuracao, mascara, niveis };

        try
        {
            int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, parametros);

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
            Console.WriteLine($"Erro ao inserir configuracao_organograma_cloud: {ex.Message}");
        }
    }
}
