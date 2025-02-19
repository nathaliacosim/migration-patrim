using MigraPatrim.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class MetodoDepreciacao
{
    private readonly PgConnect _pgConnection;

    public MetodoDepreciacao(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task MontarRegistro()
    {
        const string checkExistsQuery = @"SELECT COUNT(1) FROM metodo_depreciacao_cloud WHERE descricao = @descricao";
        const string insertQuery = @"INSERT INTO metodo_depreciacao_cloud 
                                       (id_cloud, descricao, tipo, classificacao, ativo) 
                                     VALUES 
                                       (@id_cloud, @descricao, @tipo, @classificacao, @ativo)";

        var parametros = new { 
            id_cloud = "", 
            descricao = "Linear ou Cotas Constantes",
            tipo = "DEPRECIACAO",
            classificacao = "LINEAR_OU_COTAS_CONSTANTES",
            ativo = true
        };

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
            Console.WriteLine($"Erro ao inserir metodo_depreciacao_cloud: {ex.Message}");
        }
    }
}
