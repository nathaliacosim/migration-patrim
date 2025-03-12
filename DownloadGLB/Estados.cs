using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using System.Linq;

namespace migracao-patrim.DownloadGLB;

public class Estados
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public Estados(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarEstados()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/estados";
        int offset = 0;
        const int limit = 25;

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var json_dataRet = JsonSerializer.Deserialize<EstadosGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                break;

            // Executa todas as inserções em paralelo
            await Task.WhenAll(json_dataRet.content.Select(InserirEstados));

            if (!json_dataRet.hasNext)
                break;

            offset += limit;
        }
    }

    private async Task InserirEstados(ContentEstadosGET betha)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM estados_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO estados_cloud (id_cloud, nome, uf, codigo_ibge, id_pais) VALUES (@id_cloud, @nome, @uf, @codigo_ibge, @id_pais);";

        var parameters = new
        {
            id_cloud = betha.id,
            nome = betha.nome,
            uf = betha.uf,
            codigo_ibge = betha.codigoIbge,
            id_pais = betha.pais.id,
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
            Console.WriteLine($"Erro ao inserir estado: {ex.Message}");
        }
    }
}
