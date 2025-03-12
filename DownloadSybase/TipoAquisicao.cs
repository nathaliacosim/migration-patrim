using migracao-patrim.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class TipoAquisicao
{
    private readonly PgConnect _pgConnection;

    public TipoAquisicao(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task InserirTipoAquisicao()
    {
        const string insertQuery = @"INSERT INTO tipo_aquisicao_cloud (id_cloud, descricao, classificacao) 
                                    VALUES (@id_cloud, @descricao, @classificacao)";

        var registros = new[]
        {
            new { id_cloud = string.Empty, descricao = "Dação Pagamento", classificacao = "DACAO_PAGAMENTO" },
            new { id_cloud = string.Empty, descricao = "Desmembramento", classificacao = "DESMEMBRAMENTO" },
            new { id_cloud = string.Empty, descricao = "Comodato", classificacao = "COMODATO" },
            new { id_cloud = string.Empty, descricao = "Doação", classificacao = "DOACAO" },
            new { id_cloud = string.Empty, descricao = "Desapropriação", classificacao = "DESAPROPRIACAO" },
            new { id_cloud = string.Empty, descricao = "Fabricação Própria", classificacao = "PRODUCAO_PROPRIA" },
            new { id_cloud = string.Empty, descricao = "Locação", classificacao = "LOCACAO" },
            new { id_cloud = string.Empty, descricao = "Compra", classificacao = "COMPRAS" },
            new { id_cloud = string.Empty, descricao = "Permuta", classificacao = "PERMUTA" },
            new { id_cloud = string.Empty, descricao = "Outras", classificacao = "OUTRAS_INCORPORACOES" }
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
            Console.WriteLine($"Erro ao inserir registros em tipo_aquisicao_cloud: {ex.Message}");
        }
    }
}
