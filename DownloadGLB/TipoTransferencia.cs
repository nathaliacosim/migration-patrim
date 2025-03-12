using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace migracao-patrim.DownloadGLB;

public class TipoTransferencia
{
    private readonly PgConnect _pgConnect;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public TipoTransferencia(PgConnect pgConnect, string token)
    {
        _pgConnect = pgConnect;
        _token = token;
        _httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "Authorization", $"Bearer {_token}" } }
        };
    }

    public async Task BuscarTipoTransferencia()
    {
        const string baseUrl = "https://patrimonio.betha.cloud/patrimonio-services/api/tipos-transferencia";
        int offset = 0;
        const int limit = 25;

        while (true)
        {
            string url = $"{baseUrl}?limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var json_dataRet = JsonSerializer.Deserialize<TipoTransferenciaGET>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (json_dataRet?.content == null || json_dataRet.content.Count == 0)
                break;

            // Executa todas as inserções em paralelo
            await Task.WhenAll(json_dataRet.content.Select(InserirTipoTransferencia));

            if (!json_dataRet.hasNext)
                break;

            offset += limit;
        }
    }

    private async Task InserirTipoTransferencia(ContentTipoTransferenciaGET betha)
    {
        const string checkExistsQuery = "SELECT COUNT(1) FROM tipo_transferencia_cloud WHERE id_cloud = @id_cloud;";
        const string insertQuery = "INSERT INTO tipo_transferencia_cloud (id_cloud, descricao, classificacao) VALUES (@id_cloud, @descricao, @classificacao);";

        var parameters = new
        {
            id_cloud = betha.id.ToString(),
            descricao = betha.descricao,
            classificacao = betha.classificacao.descricao
        };

        try
        {
            int count = await _pgConnect.ExecuteScalarAsync<int>(checkExistsQuery, parameters);
            if (count == 0)
            {
                await _pgConnect.ExecuteInsertAsync(insertQuery, parameters);
                Console.WriteLine($"Registro inserido: {betha.descricao}");
            }
            else
            {
                Console.WriteLine($"Registro já existe: {betha.descricao}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir tipos de tipo_transferencia_cloud: {ex.Message}");
        }
    }
}
