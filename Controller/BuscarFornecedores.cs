using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace migracao-patrim.Controller;

public class BuscarFornecedores
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public BuscarFornecedores(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarFornecs()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/fornecedores";
        int offset = 0;
        const int limit = 500;

        Console.WriteLine("🔎 Iniciando busca de fornecedores...");

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            Console.WriteLine($"📡 Buscando fornecedores... Offset: {offset}, Limite: {limit}");

            try
            {
                Console.WriteLine($"🔍 Fazendo requisição para: {url}");
                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"📜 Resposta recebida: {response.Substring(0, Math.Min(response.Length, 500))}..."); // Mostra só os primeiros 500 caracteres
                var json_dataRet = JsonSerializer.Deserialize<FornecedorGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                {
                    Console.WriteLine("✅ Busca concluída! Nenhum fornecedor encontrado na última requisição.");
                    break;
                }

                Console.WriteLine($"📥 {json_dataRet.content.Count} fornecedores recebidos. Inserindo no banco...");

                await Task.WhenAll(json_dataRet.content.Select(InserirFornecs));

                if (!json_dataRet.hasNext)
                {
                    Console.WriteLine("🚀 Todos os fornecedores foram processados!");
                    break;
                }

                offset += limit;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar fornecedores: {ex.Message}");
                Console.WriteLine($"🛠 StackTrace: {ex.StackTrace}");
                break;
            }
        }
    }

    private async Task InserirFornecs(ContentFornecedorGET betha)
    {
        if (betha == null)
        {
            Console.WriteLine("⚠️ Dados do fornecedor estão nulos. Ignorando...");
            return;
        }

        const string checkExistsQuery = "SELECT COUNT(1) FROM fornecedores_dboficial_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO fornecedores_dboficial_cloud (id_cloud, nome, cpf_cnpj, situacao) VALUES (@id_cloud, @nome, @cpf_cnpj, @situacao);";

        var parameters = new
        {
            id_cloud = betha.id.ToString(),
            nome = betha.nome.Trim(),
            cpf_cnpj = betha.cpfCnpj.Trim(),
            situacao = betha.situacao.valor.Trim()
        };

        Console.WriteLine($"📌 Processando fornecedor: {betha.nome} (ID: {betha.id}, CPF/CNPJ: {betha.cpfCnpj})");
        try
        {
            int count = await _pgConnect.ExecuteScalarAsync<int>(checkExistsQuery, parameters);
            if (count == 0)
            {
                await _pgConnect.ExecuteInsertAsync(insertQuery, parameters);
                Console.WriteLine($"✅ Fornecedor inserido: {betha.nome} (ID: {betha.id})");
            }
            else
            {
                Console.WriteLine($"🔄 Fornecedor já existe: {betha.nome} (ID: {betha.id})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao inserir fornecedor {betha.nome}: {ex.Message}");
        }
    }
}