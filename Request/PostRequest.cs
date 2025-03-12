using migracao-patrim.Request;
using System;
using System.Threading.Tasks;

namespace migracao-patrim.Request;

public class PostRequest
{
    private readonly string _token;
    private readonly string _rota;

    public PostRequest(string token, string rota)
    {
        _token = token;
        _rota = rota;
    }

    public async Task<string> Send(string dados)
    {
        string url = $"https://patrimonio.betha.cloud/patrimonio-services/{_rota}";
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Enviando requisição para: {url}");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Dados: {dados}");

        try
        {
            var requisicao = await RequisicaoHttp.PostRequisicao(_token, dados, url);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Resposta da requisição: {requisicao}");
            return requisicao;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Erro ao enviar dados: {ex.Message}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] StackTrace: {ex.StackTrace}");
            return null;
        }
    }
}
