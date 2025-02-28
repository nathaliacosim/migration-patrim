using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class TombarBens
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public TombarBens(PgConnect pgConnection, string token)
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

    public async Task<List<string>> Tombar()
    {
        var tipos = await BuscarBens();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var url_base = "https://patrimonio.betha.cloud/patrimonio-services/api/bens/" + item.id_cloud + "/tombar";

            var json_dados = new TombarBemPOST
            {
                nroPlaca = item.numero_placa,
                organograma = new OrganogramaTombarBemPOST
                {
                    id = (int)item.id_organograma
                },
                responsavel = new ResponsavelTombarBemPOST
                {
                    id = (int)item.id_responsavel
                },
                dhTombamento = DateTime.ParseExact(item.dt_inicio_deprec, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 00:00:00"
            };

            var json = JsonConvert.SerializeObject(json_dados);
            Send(json, url_base);
        }

        Console.WriteLine("FINALIZADO: Os bens estão tombados!\n\n");
        return lotesIds;
    }

    public void Send(string jsonDados, string url)
    {
        try
        {
            var requisicao = RequisicaoHttp.PostRequisicao(_token, jsonDados, url);
            Console.WriteLine(requisicao.Result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
