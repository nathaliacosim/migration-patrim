using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MigraPatrim.Request;

public class RequisicaoHttp
{
    public static async Task<string> GetRequisicao(string token, string url)
    {
        Uri urlNova = new Uri(url);
        HttpClient httpClient = new HttpClient();

        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlNova);
            HttpResponseMessage responseGet = await httpClient.SendAsync(request);
            string responseContent = await responseGet.Content.ReadAsStringAsync();

            return responseContent;
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    public static async Task<string> PostRequisicao(string token, string json, string url)
    {
        Uri urlNova = new Uri(url);
        HttpClient httpClient = new HttpClient();

        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpContent content = json != null
                   ? new StringContent(json, Encoding.UTF8, "application/json")
                   : null;

            HttpResponseMessage response = content != null
                ? await httpClient.PostAsync(urlNova, content)
                : await httpClient.PostAsync(urlNova, null);

            string responseContent = await response.Content.ReadAsStringAsync();

            // Exibir código de status no console
            Console.WriteLine($"Código de retorno: {(int)response.StatusCode} ({response.StatusCode})");

            return responseContent;
        }
        finally
        {
            httpClient.Dispose();
        }
    }


    public static async Task<string> PatchRequisicao(string token, string json, string url, Dictionary<string, string> parametros = null)
    {
        UriBuilder builder = new UriBuilder(url);

        if (parametros != null && parametros.Count > 0)
        {
            var queryString = MontarQueryString(parametros);
            builder.Query = queryString;
        }

        Uri urlNova = builder.Uri;

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), urlNova)
                {
                    Content = content
                })
                {
                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    // Exibir código de status no console
                    Console.WriteLine($"Código de retorno: {(int)response.StatusCode} ({response.StatusCode})");

                    response.EnsureSuccessStatusCode();

                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na requisição HTTP: {e.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw;
            }
        }
    }


    private static string MontarQueryString(Dictionary<string, string> parametros)
    {
        return string.Join("&", parametros.Select(param =>
            $"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}"));
    }

    public static async Task<string> DeleteRequisicao(string token, string url)
    {
        Uri urlNova = new Uri(url);

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, urlNova))
                {
                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na requisição HTTP: {e.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw;
            }
        }
    }
}

