using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace migracao-patrim.Controller;

public class BuscarResponsaveis
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public BuscarResponsaveis(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarRespons()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/responsaveis";
        int offset = 0;
        const int limit = 500;

        Console.WriteLine("🔎 Iniciando busca de responsaveis...");

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            Console.WriteLine($"📡 Buscando responsaveis... Offset: {offset}, Limite: {limit}");

            try
            {
                Console.WriteLine($"🔍 Fazendo requisição para: {url}");
                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"📜 Resposta recebida: {response.Substring(0, Math.Min(response.Length, 500))}...");
                var json_dataRet = JsonSerializer.Deserialize<ResponsavelGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                {
                    Console.WriteLine("✅ Busca concluída! Nenhum responsavel encontrado na última requisição.");
                    break;
                }

                Console.WriteLine($"📥 {json_dataRet.content.Count} responsaveis recebidos. Inserindo no banco...");

                await Task.WhenAll(json_dataRet.content.Select(InserirRespons));

                if (!json_dataRet.hasNext)
                {
                    Console.WriteLine("🚀 Todos os responsaveis foram processados!");
                    break;
                }

                offset += limit;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar responsaveis: {ex.Message}");
                Console.WriteLine($"🛠 StackTrace: {ex.StackTrace}");
                break;
            }
        }
    }

    private async Task InserirRespons(ContentResponsavelGET betha)
    {
        if (betha == null)
        {
            Console.WriteLine("⚠️ Dados do responsavel estão nulos. Ignorando...");
            return;
        }

        const string checkExistsQuery = "SELECT COUNT(1) FROM responsaveis_dboficial_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO responsaveis_dboficial_cloud (id_cloud, nome, cpf, funcao) VALUES (@id_cloud, @nome, @cpf, @funcao);";

        var parameters = new
        {
            id_cloud = betha.id.ToString(),
            nome = betha.nome.Trim(),
            cpf = betha.cpf.Trim(),
            funcao = betha.funcao?.ToString().Trim() ?? string.Empty
        };

        Console.WriteLine($"📡 Processando responsavel: {betha.nome} (ID: {betha.id}, CPF: {betha.cpf})");
        try
        {
            int count = await _pgConnect.ExecuteScalarAsync<int>(checkExistsQuery, parameters);
            if (count == 0)
            {
                await _pgConnect.ExecuteInsertAsync(insertQuery, parameters);
                Console.WriteLine($"✅ Responsavel inserido: {betha.nome} (ID: {betha.id})");
            }
            else
            {
                Console.WriteLine($"🔄 Responsavel já existe: {betha.nome} (ID: {betha.id})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao inserir responsavel {betha.nome}: {ex.Message}");
        }
    }
}