using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class GrupoBem
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public GrupoBem(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task<List<GrupoBethaDba>> BuscarGrupos()
    {
        try
        {
            using var connection = _odbcConnection.GetConnection();
            string query = @"SELECT i_conta, descricao, tipo as sigla_tipo_conta, classif as sigla_classif_conta, tipo_bem as sigla_tipo_bem, percent_deprec as percentual_depreciacao, vlr_resid as percentual_residual
                             FROM bethadba.contas";
            return (await connection.QueryAsync<GrupoBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados da tabela bethadba.contas: {ex.Message}");
            return new List<GrupoBethaDba>();
        }
    }

    public int GetIdTipoBem(string classificacao)
    {
        const string query = @"SELECT id_cloud FROM tipo_bem_cloud WHERE classificacao = @classificacao";
        return _pgConnection.GetConnection().QueryFirstOrDefault<int>(query, new { classificacao });
    }

    public int GetIdMetodoDepreciacao(string classificacao)
    {
        const string query = @"SELECT id_cloud FROM metodo_depreciacao_cloud WHERE classificacao = @classificacao";
        return _pgConnection.GetConnection().QueryFirstOrDefault<int>(query, new { classificacao });
    }

    public async Task InserirGrupos()
    {
        var grupos = await BuscarGrupos();
        foreach (var item in grupos)
        {
            var pesquisaTipo = "";
            var classificador = item.sigla_classif_conta;
            var tipo = item.sigla_tipo_conta;
            if(tipo == "R")
            {
                pesquisaTipo = "RECURSOS_NATURAIS";
            } else if(tipo == "I")
            {
                pesquisaTipo = "INTANGIVEIS";
            } else
            {
                if (classificador == 2 || classificador == 4)
                {
                    pesquisaTipo = "IMOVEIS";
                } else {
                    pesquisaTipo = "MOVEIS";
                }
            }

            var tipo_bem_id_cloud = GetIdTipoBem(pesquisaTipo);
            var metodo_depre_id_cloud = GetIdMetodoDepreciacao("LINEAR_OU_COTAS_CONSTANTES");

            const string checkExistsQuery = @"SELECT COUNT(1) FROM grupo_bem_cloud WHERE descricao = @descricao";
            const string insertQuery = @"INSERT INTO grupo_bem_cloud 
                                           (id_cloud, i_conta, descricao, id_tipo_bem, id_metodo_depreciacao,
                                            percentual_depreciacao, percentual_residual, vida_util,
                                            sigla_tipo_bem, sigla_tipo_conta, sigla_classif_conta) 
                                         VALUES 
                                           (@id_cloud, @i_conta, @descricao, @id_tipo_bem, @id_metodo_depreciacao,
                                            @percentual_depreciacao, @percentual_residual, @vida_util,
                                            @sigla_tipo_bem, @sigla_tipo_conta, @sigla_classif_conta)";

            var parametros = new { 
                id_cloud = "",
                item.i_conta,
                descricao = item.descricao.Trim(),
                id_tipo_bem = tipo_bem_id_cloud,
                id_metodo_depreciacao = metodo_depre_id_cloud,
                item.percentual_depreciacao,
                item.percentual_residual,
                vida_util = 0,
                item.sigla_tipo_bem,
                item.sigla_tipo_conta,
                item.sigla_classif_conta
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
                Console.WriteLine($"Erro ao inserir grupo_bem_cloud: {ex.Message}");
            }
        }
    }
}
