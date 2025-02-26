using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using System.Linq;

namespace MigraPatrim.DownloadGLB;

public class Logradouros
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public Logradouros(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarLogradouros()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/logradouros";
        int offset = 0;
        const int limit = 1000;

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var json_dataRet = JsonSerializer.Deserialize<LogradourosGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                break;

            // Executa todas as inserções em paralelo
            await Task.WhenAll(json_dataRet.content.Select(InserirLogradouros));

            if (!json_dataRet.hasNext)
                break;

            offset += limit;
        }
    }

    private async Task InserirLogradouros(ContentLogradourosGET betha)
    {
        // Verificar se 'betha' ou propriedades são nulas antes de usar
        if (betha == null)
        {
            Console.WriteLine("Dados do logradouro estão nulos.");
            return;
        }

        const string checkExistsQuery = "SELECT COUNT(1) FROM logradouros_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO logradouros_cloud (id_cloud, nome, cep, id_municipio, id_tipo_logradouro) VALUES (@id_cloud, @nome, @cep, @id_municipio, @id_tipo_logradouro);";

        var parameters = new
        {
            id_cloud = betha.id?.ToString() ?? string.Empty,
            nome = betha.nome ?? string.Empty,
            cep = betha.cep ?? string.Empty,
            id_municipio = betha.municipio?.id ?? 0,
            id_tipo_logradouro = betha.tipoLogradouro?.id ?? 0
        };

        try
        {
            int count = await _pgConnect.ExecuteScalarAsync<int>(checkExistsQuery, parameters);
            if (count == 0)
            {
                await _pgConnect.ExecuteInsertAsync(insertQuery, parameters);
                Console.WriteLine($"Registro inserido: {betha.nome}");
            }
            else
            {
                Console.WriteLine($"Registro já existe: {betha.nome}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir logradouro: {ex.Message}");
        }
    }
}
