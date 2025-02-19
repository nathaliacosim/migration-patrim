using System;
using System.Threading.Tasks;
using MigraPatrim.Connections;

namespace MigraPatrim.DownloadSybase;

public class TipoBem
{
    private readonly PgConnect _pgConnection;

    public TipoBem(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task InserirRegistrosPadrao()
    {
        const string insertQuery = @"INSERT INTO tipo_bem_cloud (id_cloud, descricao, classificacao) 
                                    VALUES (@id_cloud, @descricao, @classificacao)";

        var registros = new[]
        {
            new { id_cloud = "", descricao = "Imóveis", classificacao = "IMOVEIS" },
            new { id_cloud = "", descricao = "Intangíveis", classificacao = "INTANGIVEIS" },
            new { id_cloud = "", descricao = "Móveis", classificacao = "MOVEIS" },
            new { id_cloud = "", descricao = "Recursos Naturais", classificacao = "RECURSOS_NATURAIS" }
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
            Console.WriteLine($"Erro ao inserir registros em tipo_bem_cloud: {ex.Message}");
        }
    }
}

