using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadSybase;

public class Localizacao
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Localizacao(OdbcConnect odbcConnect, PgConnect pgConnect)
    {
        _odbcConnection = odbcConnect;
        _pgConnection = pgConnect;
    }

    public async Task<List<LocalizacaoBethaDba>> BuscarLocalizacaoBem()
    {
        const string query = "SELECT * FROM bethadba.localizacoes;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<LocalizacaoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<LocalizacaoBethaDba>();
        }
    }

    public string SetMovimento(string sigla)
    {
        if (string.IsNullOrWhiteSpace(sigla)) return "OUTROS";

        switch (sigla?.ToUpper())
        {
            case "C": return "CENTRO_CUSTO";
            case "R": return "RESPONSAVEL";
            case "T": return "TRANSFERENCIA_COMODATO";
            case "E": return "RECEBIMENTO_COMODATO";
            case "L": return "TRANSFERENCIA_LOCACAO";
            case "O": return "RECEBIMENTO_LOCACAO";
            case "S": return "TRANSFERENCIA_CESSAO";
            case "A": return "RECEBIMENTO_CESSAO";
            case "N": return "CONTA";
            case "F": return "LOCALIZACAO_FISICA";
            case "X": return "TRANSFERENCIA_ENTRE_ENTIDADES";
            default: return "OUTROS";
        }
    }


    public async Task InsertIntoLocalizacaoBem()
    {
        var dados = await BuscarLocalizacaoBem();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM transferencias_cloud WHERE i_localizacao = @i_localizacao";
            const string insertQuery = @"INSERT INTO transferencias_cloud 
                                           (i_localizacao, i_bem, data_movimento, sigla_movimento, tipo_movimento,
                                            codigo_novo1, codigo_novo2, codigo_antigo1, codigo_antigo2, historico)
                                         VALUES 
                                            (@i_localizacao, @i_bem, @data_movimento, @sigla_movimento, @tipo_movimento,
                                             @codigo_novo1, @codigo_novo2, @codigo_antigo1, @codigo_antigo2, @historico)";

            var parametros = new
            {
                item.i_localizacao,
                item.i_bem,
                data_movimento = item.data_transf,
                sigla_movimento = item.tipo,
                tipo_movimento = SetMovimento(item.tipo),
                codigo_novo1 = item.novo_cod1,
                codigo_novo2 = item.novo_cod2,
                codigo_antigo1 = item.cod_anterior1,
                codigo_antigo2 = item.cod_anterior2,
                historico = item.historico?.Trim() ?? ""
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_localizacao });

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
                Console.WriteLine($"Erro ao inserir transferencias_cloud: {ex.Message}");
            }
        }
    }
}
