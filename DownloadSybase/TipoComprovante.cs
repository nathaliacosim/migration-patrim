using System;
using migracao-patrim.Connections;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class TipoComprovante
{
    private readonly PgConnect _pgConnection;

    public TipoComprovante(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task InserirRegistrosPadrao()
    {
        const string insertQuery = @"INSERT INTO tipo_comprovante_cloud (id_cloud, descricao, tipo) 
                                    VALUES (@id_cloud, @descricao, @tipo)";

        var registros = new[]
        {
            new { id_cloud = "", descricao = "Notal Fiscal", tipo = "NOTA_FISCAL" },
            new { id_cloud = "", descricao = "Cupom Fiscal", tipo = "CUPOM_FISCAL" }
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
            Console.WriteLine($"Erro ao inserir registros em tipo_comprovante_cloud: {ex.Message}");
        }
    }
}
