using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarBens
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarBens(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/bens");
    }

    public async Task<List<Models.ModelPostgres.Bem>> BuscarBens()
    {
        const string query = "SELECT * FROM public.bens_cloud;";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.Bem>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar bens_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Bem>();
        }
    }

    public async Task<List<string>> EnviarBensCloud()
    {
        var tipos = await BuscarBens();
        var lotesIds = new List<string>();
        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new BensPOST
            {
                numeroRegistro = item.i_bem.ToString(),
                numeroPlaca = item.numero_placa,
                numeroComprovante = item.documento,
                descricao = item.descricao.Trim().ToUpper(),
                dataInclusao = "2025-01-01",
                dataAquisicao = DateTime.ParseExact(item.data_aquis, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"),
                consomeCombustivel = false,
                numeroAnoEmpenho = item.nr_empenho != null && item.ano_empenho != null ? new List<NumeroAnoEmpenhoBensPOST>
                {
                    new NumeroAnoEmpenhoBensPOST
                    {
                        descricao = (item.nr_empenho+"/"+item.ano_empenho),
                    }
                } : null,
                numeroAnoProcesso = item.i_processo != null && item.i_ano_proc != null ? new NumeroAnoProcessoBensPOST
                {
                    descricao = item.i_processo+"/"+ item.i_ano_proc,
                } : null,
                numeroAnoSolicitacao = null,
                tipoBem = new TipoBemBensPOST
                {
                    id = item.id_tipo_bem,
                },
                grupoBem = new GrupoBemBensPOST
                {
                    id = item.id_grupo_bem,
                },
                especieBem = new EspecieBemBensPOST
                {
                    id = item.id_especie_bem,
                },
                tipoUtilizacaoBem = null/*new TipoUtilizacaoBemBensPOST
                {
                    id = item.id_tipo_utilizacao,
                }*/,
                tipoAquisicao = new TipoAquisicaoBensPOST
                {
                    id = item.id_tipo_aquisicao,
                },
                fornecedor = item.i_fornec != null ? new FornecedorBensPOST
                {
                    id = item.id_fornecedor,
                } : null,
                responsavel = item.i_respons != null ? new ResponsavelBensPOST
                {
                    id = item.id_responsavel,
                } : null,
                estadoConservacao = new EstadoConservacaoBensPOST
                {
                    id = item.id_estado_conservacao,
                },
                tipoComprovante = item.id_tipo_comprovante != null ? new TipoComprovanteBensPOST
                {
                    id = item.id_tipo_comprovante,
                }: null,
                organograma = new OrganogramaBensPOST
                {
                    id = item.id_organograma,
                },
                situacaoBem = new SituacaoBemBensPOST
                {
                    descricao = "Em Edição",
                    valor = "EM_EDICAO"
                },
                bemValor = new BemValorBensPOST
                {
                    metodoDepreciacao = item.perc_deprec_anual > 0 ? new MetodoDepreciacaoBensPOST
                    {
                        id = item.id_metodo_depreciacao
                    } : null,
                    moeda = new MoedaBensPOST
                    {
                        id = 8,
                        nome = "R$ - Real (1994)",
                        sigla = "R$",
                        dtCotacao = "1994-07-01",
                        fatorConversao = 2750,
                        formaCalculo = "DIVIDIR"
                    },
                    vlAquisicao = item.valor_aquis,
                    vlAquisicaoConvertido = item.valor_aquis,
                    vlResidual = 0,
                    saldoDepreciar = item.valor_aquis,
                    vlDepreciado = 0,
                    vlDepreciavel = item.valor_aquis,
                    vlLiquidoContabil = item.valor_aquis,
                    taxaDepreciacaoAnual = item.perc_deprec_anual > 0 ? item.perc_deprec_anual : null,
                    dtInicioDepreciacao = item.perc_deprec_anual > 0 ? DateTime.ParseExact(item.dt_inicio_deprec, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : null,
                    anosVidaUtil = item.perc_deprec_anual > 0 ? item.vida_util : null                    
                }
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar bens_cloud {item.id}: {errorMessage}\n");
                }
                else
                {
                    lotesIds.Add(resposta);
                    Console.WriteLine($"Atualizando ID Cloud: {item.id} -> {resposta}");
                    await AtualizarIdCloud(item.id, resposta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar bens_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: bens_cloud.\n\n");
        return lotesIds;
    }

    private static bool TryParseErrorResponse(string resposta, out string message)
    {
        try
        {
            var erro = JsonConvert.DeserializeObject<Dictionary<string, string>>(resposta);
            return erro.TryGetValue("message", out message);
        }
        catch (JsonException)
        {
            message = null;
            return false;
        }
    }

    public async Task AtualizarIdCloud(int id, string idCloud)
    {
        const string query = "UPDATE bens_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para bens_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para bens_cloud {id}: {ex.Message}");
        }
    }
}
