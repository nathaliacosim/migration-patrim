using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Models.ModelPostgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarMovimentos
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;
    private readonly HttpClient _httpClient;

    public EnviarMovimentos(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
    }

    public List<string> ObterDatasOrdenadas()
    {
        var datas = new List<string>();
        DateTime dataInicial = new DateTime(2016, 12, 1);
        DateTime dataAtual = DateTime.Now;

        while (dataInicial <= dataAtual)
        {
            datas.Add(dataInicial.ToString("yyyy-MM-dd"));
            dataInicial = dataInicial.AddDays(1);
        }

        return datas;
    }

    private async Task<List<T>> BuscarDados<T>(string query, object parametros)
    {
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<T>(query, parametros)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar dados: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task<List<BaixaBem>> BuscaBaixas(string data) =>
        await BuscarDados<BaixaBem>("SELECT * FROM baixas_cloud WHERE data_baixa = @data", new { data });

    public async Task<List<DepreciacaoBem>> BuscaDepreciacoes(string data) =>
        await BuscarDados<DepreciacaoBem>("SELECT * FROM depreciacoes_cloud WHERE data_depreciacao = @data", new { data });

    public async Task<List<TransferenciaBem>> BuscaTransferencias(string data) =>
        await BuscarDados<TransferenciaBem>("SELECT * FROM transferencias_cloud WHERE data_movimento = @data", new { data });

    public async Task<bool> EnviarParaApi(bool atualizar, Dictionary<string, string> dadosAtualiza, string url, object body, int tentativas = 3)
    {
        try
        {
            Console.WriteLine($"🚀 Enviando requisição para {url}...");
            var jsonContent = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ Envio bem-sucedido para {url}!");
                if (atualizar)
                {
                    string resposta = await response.Content.ReadAsStringAsync();
                    await AtualizarIdCloud(dadosAtualiza["tabela"], int.Parse(dadosAtualiza["id_registro"]), resposta);
                }
                return true;
            }

            Console.WriteLine($"❌ Erro {response.StatusCode} ao enviar para {url}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar requisição para {url}: {ex.Message}");
            if (tentativas > 1)
            {
                Console.WriteLine($"⏳ Tentando novamente ({4 - tentativas}/3)...");
                await Task.Delay(2000);
                return await EnviarParaApi(atualizar, dadosAtualiza, url, body, tentativas - 1);
            }
        }
        return false;
    }

    public async Task CriarPacote(string data,
                                  List<DepreciacaoBem> depreciacoes,
                                  List<TransferenciaBem> transferencias,
                                  List<BaixaBem> baixas)
    {
        Console.WriteLine($"📦 Criando pacote para a data {data}...");

        // Processa Depreciações
        foreach (var dep in depreciacoes)
        {
            string url = $"https://patrimonio.betha.cloud/patrimonio-services/api/depreciacoes/{dep.id_cloud_depreciacao}/bens/";
            var body = new DepreciacaoBemPOST
            {
                depreciacao = new DepreciacaoDepreciacaoBemPOST
                {
                    id = dep.id_cloud_depreciacao
                },
                bem = new BemDepreciacaoBemPOST
                {
                    id = dep.id_cloud_bem
                },
                vlDepreciado = dep.valor_depreciado,
                notaExplicativa = ""
            };

            Dictionary<string, string> mapaAtualizacao = new Dictionary<string, string>
            {
                { "tabela", "D" },
                { "id_registro", dep.id.ToString() }
            };

            await EnviarParaApi(true, mapaAtualizacao, url, body);
        }

        // Processa Transferências
        foreach (var transf in transferencias)
        {
            string url = $"https://patrimonio.betha.cloud/patrimonio-services/api/transferencias/{transf.id_cloud_transferencia}/bens";
            var body = new TransferenciaBemPOST
            {
                bem = new BemTransferenciaBemPOST
                {
                    id = transf.id_cloud_bem
                },
                transferencia = new TransferenciaTransferenciaBemPOST
                {
                    id = transf.id_cloud_transferencia
                }
            };

            Dictionary<string, string> mapaAtualizacao = new Dictionary<string, string>
            {
                { "tabela", "T" },
                { "id_registro", transf.id.ToString() }
            };

            await EnviarParaApi(true, mapaAtualizacao, url, body);
        }

        // Processa Baixas
        foreach (var baixa in baixas)
        {
            string url = $"https://patrimonio.betha.cloud/patrimonio-services/api/baixas/{baixa.id_cloud_baixa}/bens/";
            var body = new BaixaBensPOST
            {
                notaExplicativa = baixa.nota_explicativa ?? "",
                baixa = new BaixaBaixaBensPOST
                {
                    id = baixa.id_cloud_baixa
                },
                bem = new BemBaixaBensPOST
                {
                    id = baixa.id_cloud_bem
                }
            };

            Dictionary<string, string> mapaAtualizacao = new Dictionary<string, string>
            {
                { "tabela", "B" },
                { "id_registro", baixa.id.ToString() }
            };

            await EnviarParaApi(true, mapaAtualizacao, url, body);
        }

        Console.WriteLine($"✅ Todos os bens da data {data} foram adicionados aos pacotes.");

        // Agora finaliza os pacotes
        await FinalizarPacote(depreciacoes, transferencias, baixas);
    }

    private async Task<bool> TodosOsDiasDoMesForamEnviados(string mesAno)
    {
        const string query = "SELECT DISTINCT to_char(TO_DATE(data_depreciacao, 'YYYY-MM-DD'), 'YYYY-MM') as mes_ano FROM depreciacoes_cloud";
        using var connection = _pgConnection.GetConnection();
        var mesesEnviados = await connection.QueryAsync<string>(query);
        return mesesEnviados.Contains(mesAno);
    }

    private async Task FinalizarPacote(List<DepreciacaoBem> depreciacoes,
                                       List<TransferenciaBem> transferencias,
                                       List<BaixaBem> baixas)
    {
        Console.WriteLine($"🚀 Finalizando pacotes...");

        if (depreciacoes.Any() && DateTime.TryParseExact(depreciacoes.First().data_depreciacao, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime data))
        {
            string mesAno = data.ToString("yyyy-MM");
            if (await TodosOsDiasDoMesForamEnviados(mesAno))
            {
                foreach (var dep in depreciacoes)
                {
                    string url = $"https://patrimonio.betha.cloud/patrimonio-services/api/depreciacoes/{dep.id_cloud_depreciacao}/finalizar/";
                    await EnviarParaApi(false, new Dictionary<string, string>(), url, new { mensagem = "Finalização do pacote de depreciação" });
                }
            }
            else
            {
                Console.WriteLine($"⏳ Ainda existem dias pendentes para finalização do mês {mesAno}. Aguardando conclusão.");
            }
        }

        foreach (var transf in transferencias)
        {
            string url = $"https://patrimonio.betha.cloud/patrimonio-services/api/transferencia/{transf.id_cloud_transferencia}/finalizar/";
            await EnviarParaApi(false, new Dictionary<string, string>(), url, new { mensagem = "Finalização do pacote de transferência" });
        }

        foreach (var baixa in baixas)
        {
            string url = $"https://patrimonio.betha.cloud/patrimonio-services/api/baixas/{baixa.id_cloud_baixa}/finalizada/";
            await EnviarParaApi(false, new Dictionary<string, string>(), url, new { mensagem = "Finalização do pacote de baixa" });
        }

        Console.WriteLine("🎉 Todos os pacotes foram finalizados!");
    }

    public async Task ProcessarPacotes()
    {
        var datas = ObterDatasOrdenadas();
        foreach (var data in datas)
        {
            Console.WriteLine($"📅 Processando movimentações para {data}...");

            var depreciacoes = await BuscaDepreciacoes(data);
            var transferencias = await BuscaTransferencias(data);
            var baixas = await BuscaBaixas(data);

            if (depreciacoes.Any() || transferencias.Any() || baixas.Any())
            {
                await CriarPacote(data, depreciacoes, transferencias, baixas);
            }
            else
            {
                Console.WriteLine($"⚠️ Nenhuma movimentação encontrada para {data}. Pulando...");
            }

            await Task.Delay(1000);
        }

        Console.WriteLine("🎉 Processamento de pacotes finalizado!");
    }

    public async Task<string> AtualizarIdCloud(string tipo, int id, string idCloud)
    {
        string query = tipo switch
        {
            "D" => "UPDATE depreciacoes_cloud SET id_cloud = @idCloud WHERE id = @id RETURNING id_cloud",
            "T" => "UPDATE transferencias_cloud SET id_cloud = @idCloud WHERE id = @id RETURNING id_cloud",
            "B" => "UPDATE baixas_cloud SET id_cloud = @idCloud WHERE id = @id RETURNING id_cloud",
            _ => throw new ArgumentException("Tipo de movimento inválido.")
        };

        try
        {
            using var connection = _pgConnection.GetConnection();
            var idAtualizado = await connection.ExecuteScalarAsync<string>(query, new { id, idCloud });
            if (!string.IsNullOrEmpty(idAtualizado))
            {
                Console.WriteLine($"🔄 ID Cloud atualizado com sucesso para {tipo} {id}: {idAtualizado} ✔️");
                return idAtualizado;
            }
            else
            {
                Console.WriteLine($"❌ Não foi possível atualizar o ID Cloud para {tipo} {id}");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao atualizar ID Cloud para {tipo} {id}: {ex.Message}");
            return string.Empty;
        }
    }
}
