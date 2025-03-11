using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class BuscarOrganogramas
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public BuscarOrganogramas(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarOrganog()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/organogramas";
        int offset = 0;
        const int limit = 500;

        Console.WriteLine("🔎 Iniciando busca de organogramas...");

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            Console.WriteLine($"📡 Buscando organogramas... Offset: {offset}, Limite: {limit}");

            try
            {
                Console.WriteLine($"🔍 Fazendo requisição para: {url}");
                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"📜 Resposta recebida: {response.Substring(0, Math.Min(response.Length, 500))}...");
                var json_dataRet = JsonSerializer.Deserialize<OrganogramaGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                {
                    Console.WriteLine("✅ Busca concluída! Nenhum organograma encontrado na última requisição.");
                    break;
                }

                Console.WriteLine($"📥 {json_dataRet.content.Count} organogramas recebidos. Inserindo no banco...");

                await Task.WhenAll(json_dataRet.content.Select(InserirOrganog));

                if (!json_dataRet.hasNext)
                {
                    Console.WriteLine("🚀 Todos os organogramas foram processados!");
                    break;
                }

                offset += limit;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar organogramas: {ex.Message}");
                Console.WriteLine($"🛠 StackTrace: {ex.StackTrace}");
                break;
            }
        }
    }

    private async Task InserirOrganog(ContentOrganogramaGET betha)
    {
        if (betha == null)
        {
            Console.WriteLine("⚠️ Dados do organograma estão nulos. Ignorando...");
            return;
        }

        if (betha.configuracaoOrganograma.id != 17351) return;

        const string checkExistsQuery = "SELECT COUNT(1) FROM organograma_dboficial_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO organograma_dboficial_cloud (id_cloud, descricao, numero, nivel, config_id, orgao, unidade, centro_custo) VALUES (@id_cloud, @descricao, @numero, @nivel, @config_id, @orgao, @unidade, @centro_custo);";

        var numeroOrganograma = betha.numeroOrganograma.Trim();

        if (numeroOrganograma.Length != 10)
        {
            Console.WriteLine($"⚠️ Formato inesperado de numeroOrganograma: {numeroOrganograma}. Ignorando...");
            return;
        }

        var parameters = new
        {
            id_cloud = betha.id.ToString(),
            descricao = betha.descricao.Trim(),
            numero = numeroOrganograma,
            nivel = betha.nivel.ToString(),
            config_id = betha.configuracaoOrganograma.id,
            orgao = numeroOrganograma.Substring(0, 2),       // 2 primeiros dígitos → Órgão
            unidade = numeroOrganograma.Substring(2, 3),     // 3 próximos dígitos → Unidade
            centro_custo = numeroOrganograma.Substring(5, 5) // 5 últimos dígitos → Centro de Custo
        };

        Console.WriteLine($"📡 Processando organograma: {betha.descricao} (ID: {betha.id}, Numero: {betha.numeroOrganograma})");
        try
        {
            int count = await _pgConnect.ExecuteScalarAsync<int>(checkExistsQuery, parameters);
            if (count == 0)
            {
                await _pgConnect.ExecuteInsertAsync(insertQuery, parameters);
                Console.WriteLine($"✅ Organograma inserido: {betha.numeroOrganograma} (ID: {betha.id})");
            }
            else
            {
                Console.WriteLine($"🔄 Organograma já existe: {betha.numeroOrganograma} (ID: {betha.id})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao inserir organograma {betha.numeroOrganograma}: {ex.Message}");
        }
    }
}