using System;
using System.Threading.Tasks;
using migracao-patrim.Connections;

namespace migracao-patrim.DownloadSybase;

public class TipoUtilizacao
{
    private readonly PgConnect _pgConnection;

    public TipoUtilizacao(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task InserirRegistrosPadrao()
    {
        const string insertQuery = @"INSERT INTO tipo_utilizacao_cloud (id_cloud, descricao, classificacao) 
                                    VALUES (@id_cloud, @descricao, @classificacao)";

        var registros = new[]
        {
            new { id_cloud = "", descricao = "Dominicais", classificacao = "DOMINICAIS" },
            new { id_cloud = "", descricao = "Uso comum", classificacao = "USO_COMUM_POVO" },
            new { id_cloud = "", descricao = "Uso especial", classificacao = "USO_ESPECIAL" }
        };

        try
        {
            foreach (var registro in registros)
            {
                _pgConnection.Execute(insertQuery, registro);
            }
            Console.WriteLine("Registros inseridos com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir registros em tipo_utilizacao_cloud: {ex.Message}");
        }
    }
}
