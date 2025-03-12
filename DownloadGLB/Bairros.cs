using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadGLB;

public class Bairros
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public Bairros(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarBairros()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/bairros";
        int offset = 0;
        const int limit = 25;

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var json_dataRet = JsonSerializer.Deserialize<BairroGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                break;

            // Executa todas as inserções em paralelo
            await Task.WhenAll(json_dataRet.content.Select(InserirBairros));

            if (!json_dataRet.hasNext)
                break;

            offset += limit;
        }
    }

    private async Task InserirBairros(ContentBairroGET betha)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM bairros_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO bairros_cloud (id_cloud, nome, id_municipio) VALUES (@id_cloud, @nome, @id_municipio);";

        var parameters = new
        {
            id_cloud = betha.id.ToString(),
            nome = betha.nome.ToUpper(),
            id_municipio = betha.municipio.id
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
            Console.WriteLine($"Erro ao inserir bairro: {ex.Message}");
        }
    }
}
