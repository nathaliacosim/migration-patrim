using System;
using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelSybase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase;

public class CadastroBem
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public CadastroBem(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task<List<BemBethaDba>> BuscarBens()
    {
        const string query = "SELECT * FROM bethadba.bens;";
        try
        {
            using var connection = _odbcConnection.GetConnection();
            return (await connection.QueryAsync<BemBethaDba>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
            return new List<BemBethaDba>();
        }
    }

    public async Task InsertIntoBens()
    {
        var dados = await BuscarBens();
        foreach (var item in dados)
        {
            const string checkExistsQuery = @"SELECT COUNT(1) FROM bens_cloud WHERE i_bem = @i_bem";
            const string insertQuery = @"INSERT INTO bens_cloud 
                                            (id_cloud, i_bem, i_conta, i_custo, i_unidade, i_respons, i_fornec, 
                                            i_tipo_natur, descricao, data_aquis, documento, fornecedor, 
                                            valor_aquis, i_moeda, valor_original, valor_reav, valor_depr, tipo_aquis, 
                                            data_garant, data_proxrev, nr_empenho, situacao, historico, uso, 
                                            est_cons, ano_empenho, i_ano_proc, i_processo, data_processo, liquid_compras, 
                                            codigo_localizacao, codigo_intervencao, numero_placa, numero_termo, 
                                            data_termo, data_prazo, valor_residual, valor_depreciavel, dt_inicio_deprec, 
                                            perc_deprec_anual, utiliza_exprecnatur, i_localiz_fis, i_entidades, 
                                            i_detalhamentos_bens, utilizacao, origem, i_tipo_documento, vida_util, 
                                            id_tipo_bem, id_grupo_bem, id_especie_bem, id_tipo_utilizacao, 
                                            id_tipo_aquisicao, id_fornecedor, id_responsavel, id_estado_conservacao, 
                                            id_tipo_comprovante, id_organograma, id_localizacao_fisica, 
                                            id_metodo_depreciacao) 
                                         VALUES 
                                            (@id_cloud, @i_bem, @i_conta, @i_custo, @i_unidade, @i_respons, 
                                            @i_fornec, @i_tipo_natur, @descricao, @data_aquis, @documento, 
                                            @fornecedor, @valor_aquis, @i_moeda, @valor_original, @valor_reav, 
                                            @valor_depr, @tipo_aquis, @data_garant, @data_proxrev, @nr_empenho, 
                                            @situacao, @historico, @uso, @est_cons, @ano_empenho, @i_ano_proc, 
                                            @i_processo, @data_processo, @liquid_compras, @codigo_localizacao, 
                                            @codigo_intervencao, @numero_placa, @numero_termo, @data_termo, 
                                            @data_prazo, @valor_residual, @valor_depreciavel, @dt_inicio_deprec, 
                                            @perc_deprec_anual, @utiliza_exprecnatur, @i_localiz_fis, @i_entidades, 
                                            @i_detalhamentos_bens, @utilizacao, @origem, @i_tipo_documento, 
                                            @vida_util, @id_tipo_bem, @id_grupo_bem, @id_especie_bem, 
                                            @id_tipo_utilizacao, @id_tipo_aquisicao, @id_fornecedor, @id_responsavel, 
                                            @id_estado_conservacao, @id_tipo_comprovante, @id_organograma, 
                                            @id_localizacao_fisica, @id_metodo_depreciacao)";

            var parametros = new
            {
                id_cloud = "",
                item.i_bem,
                item.i_conta,
                item.i_custo,
                item.i_unidade,
                item.i_respons,
                item.i_fornec,
                item.i_tipo_natur,
                descricao = item.descricao.Trim(),
                item.data_aquis,
                item.documento,
                item.fornecedor,
                item.valor_aquis,
                item.i_moeda,
                item.valor_original,
                item.valor_reav,
                item.valor_depr,
                item.tipo_aquis,
                item.data_garant,
                item.data_proxrev,
                item.nr_empenho,
                item.situacao,
                item.historico,
                item.uso,
                item.est_cons,
                item.ano_empenho,
                item.i_ano_proc,
                item.i_processo,
                item.data_processo,
                item.liquid_compras,
                item.codigo_localizacao,
                item.codigo_intervencao,
                item.numero_placa,
                item.numero_termo,
                item.data_termo,
                item.data_prazo,
                item.valor_residual,
                item.valor_depreciavel,
                item.dt_inicio_deprec,
                item.perc_deprec_anual,
                item.utiliza_exprecnatur,
                item.i_localiz_fis,
                item.i_entidades,
                item.i_detalhamentos_bens,
                item.utilizacao,
                item.origem,
                item.i_tipo_documento,
                item.vida_util,
                id_tipo_bem = (int?)null,
                id_grupo_bem = (int?)null,
                id_especie_bem = (int?)null,
                id_tipo_utilizacao = (int?)null,
                id_tipo_aquisicao = (int?)null,
                id_fornecedor = (int?)null,
                id_responsavel = (int?)null,
                id_estado_conservacao = (int?)null,
                id_tipo_comprovante = (int?)null,
                id_organograma = (int?)null,
                id_localizacao_fisica = (int?)null,
                id_metodo_depreciacao = (int?)null
            };

            try
            {
                int count = _pgConnection.ExecuteScalar<int>(checkExistsQuery, new { item.i_bem });

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
                Console.WriteLine($"Erro ao inserir bens_cloud: {ex.Message}");
            }
        }
    }

}