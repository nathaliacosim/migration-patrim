using System;
using MigraPatrim.Connections;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class EstadoConservacao
{
    private readonly PgConnect _pgConnect;

    public EstadoConservacao(PgConnect pgConnect)
    {
        _pgConnect = pgConnect;
    }

    public async Task InsertIntoEstadoConservacao()
    {
        const string insertQuery = @"INSERT INTO estado_conservacao_cloud (id_cloud, descricao, sigla) 
                                     VALUES (@id_cloud, @descricao, @sigla);";

        // Estado de conservação do bem (O=Ótimo, B=Bom, R=Regular, U=Ruim, P=Péssimo)
        var registros = new[]
        {
            new { id_cloud = string.Empty, descricao = "Ótimo", sigla = "O" },
            new { id_cloud = string.Empty, descricao = "Bom", sigla = "B" },
            new { id_cloud = string.Empty, descricao = "Regular", sigla = "R" },
            new { id_cloud = string.Empty, descricao = "Ruim", sigla = "U" },
            new { id_cloud = string.Empty, descricao = "Péssimo", sigla = "P" }
        };

        try
        {
            foreach (var registro in registros)
            {
                _pgConnect.Execute(insertQuery, registro);
            }
            Console.WriteLine("Registros inseridos com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir registros em estado_conservacao_cloud: {ex.Message}");
        }
    }
}
