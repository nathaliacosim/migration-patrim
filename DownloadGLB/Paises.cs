using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using System.Linq;

namespace MigraPatrim.DownloadGLB;

public class Paises
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public Paises(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarPaises()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/paises";
        int offset = 0;
        const int limit = 25;

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var json_dataRet = JsonSerializer.Deserialize<PaisesGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                break;

            // Executa todas as inserções em paralelo
            await Task.WhenAll(json_dataRet.content.Select(InserirPaises));

            if (!json_dataRet.hasNext)
                break;

            offset += limit;
        }
    }

    private async Task InserirPaises(ContentPaisesGET betha)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM paises_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO paises_cloud (id_cloud, nome) VALUES (@id_cloud, @nome);";

        var parameters = new
        {
            id_cloud = betha.id,
            nome = betha.nome
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
            Console.WriteLine($"Erro ao inserir país: {ex.Message}");
        }
    }
}
