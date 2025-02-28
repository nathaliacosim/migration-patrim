using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class AguardarTombamento
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;
        
    public AguardarTombamento(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task<List<Models.ModelPostgres.Bem>> BuscarBens()
    {
        const string query = "SELECT * FROM public.bens_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.Bem>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar bens_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Bem>();
        }
    }

    public async Task<List<string>> AguardandoTombamento()
    {
        var tipos = await BuscarBens();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            //var url_base = "https://patrimonio.betha.cloud/patrimonio-services/api/bens/" + item.id_cloud + "/aguardarTombamento";
            var url_base = "https://patrimonio.betha.cloud/patrimonio-services/api/bens/" + item.id_cloud + "/desfazerTombamento";
            Send(url_base);
        }

        Console.WriteLine("FINALIZADO: Os bens estão prontos para tombamento!\n\n");
        return lotesIds;
    }

    public void Send(string url)
    {
        try
        {
            var requisicao = RequisicaoHttp.PostRequisicao(_token, null, url);
            Console.WriteLine(requisicao.Result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
