using System;
using Dapper;
using System.Threading.Tasks;
using migracao-patrim.Connections;
using System.Collections.Generic;
using migracao-patrim.Models.ModelSybase;
using migracao-patrim.Models.ModelCloud;

namespace migracao-patrim.DownloadSybase;

public class Exercicios
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Exercicios(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;

        Task task = InsertIntoExercicios();
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

    public int GetIdCloudConfiguracaoOrganograma(int exerc)
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            string query = @"SELECT id_cloud FROM public.configuracao_organograma_cloud WHERE descricao LIKE @exerc LIMIT 1";
            var idCloud = connection.QueryFirstOrDefault<string>(query, new { exerc = "%" + exerc + "%" });

            if (string.IsNullOrEmpty(idCloud))
            {
                return 0;
            }

            return int.TryParse(idCloud, out var id) ? id : 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela public.configuracao_organograma_cloud: {ex.Message}");
            return 0;
        }
    }

    public async Task InsertIntoExercicios()
    {
        var listaExercicios = await BuscarExercicios();
        foreach (var exercicio in listaExercicios)
        {
            var idConfiguracao = GetIdCloudConfiguracaoOrganograma(exercicio.i_exercicio);
            Console.WriteLine($"Exercício: {exercicio.i_exercicio}");

            const string checkExistsQuery = @"SELECT COUNT(1) FROM exercicio_cloud WHERE exercicio = @exercicio";
            const string insertQuery = @"INSERT INTO exercicio_cloud (id_cloud, id_configuracao, exercicio) 
                                         VALUES (@id_cloud, @id_configuracao, @exercicio)";

            var parametros = new
            {
                id_cloud = "",
                id_configuracao = idConfiguracao,
                exercicio = exercicio.i_exercicio
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { exercicio = exercicio.i_exercicio });

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
                Console.WriteLine($"Erro ao inserir exercicio_cloud: {ex.Message}");
            }
        }
    }
}
