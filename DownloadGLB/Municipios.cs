using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using System.Linq;

namespace MigraPatrim.DownloadGLB;

public class Municipios
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public Municipios(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarMunicipios()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/municipios";
        int offset = 0;
        const int limit = 25;

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var json_dataRet = JsonSerializer.Deserialize<MunicipiosGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                break;

            // Executa todas as inserções em paralelo
            await Task.WhenAll(json_dataRet.content.Select(InserirMunicipios));

            if (!json_dataRet.hasNext)
                break;

            offset += limit;
        }
    }

    private async Task InserirMunicipios(ContentMunicipiosGET betha)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM municipios_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO municipios_cloud (id_cloud, nome, id_estado, latitude, longitude, codigo_ibge, codigo_tribunal, codigo_siafi) VALUES (@id_cloud, @nome, @id_estado, @latitude, @longitude, @codigo_ibge, @codigo_tribunal, @codigo_siafi);";

        var parameters = new
        {
            id_cloud = betha.id,
            betha.nome,
            id_estado = betha.estado.id,
            betha.latitude,
            betha.longitude,
            codigo_ibge = betha.codigoIbge ?? 0,
            codigo_tribunal = betha.codigoTribunal ?? 0,
            codigo_siafi = betha.codigoSiafi ?? 0
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
            Console.WriteLine($"Erro ao inserir município: {ex.Message}");
        }
    }
}
