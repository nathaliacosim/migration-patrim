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
        const string query = "SELECT * FROM public.bens_cloud;";
        try
        {
            using var connection = _pgConnection.GetConnection();
            Console.WriteLine("🔍 Buscando bens no banco de dados...");
            var bens = (await connection.QueryAsync<Models.ModelPostgres.Bem>(query)).AsList();
            Console.WriteLine($"✅ {bens.Count} bens encontrados!");
            return bens;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar bens_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.Bem>();
        }
    }

    public async Task<List<string>> Tombar()
    {
        Console.WriteLine("📦 Iniciando o tombamento dos bens...");
        var tipos = await BuscarBens();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var data_tombamento = DateTime.ParseExact((item?.dt_inicio_deprec ?? item.data_aquis), "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + " 00:00:00";
            Console.WriteLine($"🛠️ Tombando bem: 📌 nroPlaca: {item.numero_placa} | 🏢 id_organograma: {item.id_organograma} | 👤 id_responsavel: {item.id_responsavel} | 📅 Data: {data_tombamento}");

            var url_base = "https://patrimonio.betha.cloud/patrimonio-services/api/bens/" + item.id_cloud + "/tombar";
            var json_dados = new TombarBemPOST
            {
                nroPlaca = item?.numero_placa,
                organograma = new OrganogramaTombarBemPOST
                {
                    id = (int)item.id_organograma
                },
                responsavel = new ResponsavelTombarBemPOST
                {
                    id = (int)item.id_responsavel
                },
                dhTombamento = data_tombamento
            };

            var json = JsonConvert.SerializeObject(json_dados);
            Send(json, url_base);
        }

        Console.WriteLine("🎉 FINALIZADO: Os bens estão tombados com sucesso! ✅\n\n");
        return lotesIds;
    }

    public void Send(string jsonDados, string url)
    {
        try
        {
            Console.WriteLine("📡 Enviando requisição para tombamento...");
            var requisicao = RequisicaoHttp.PostRequisicao(_token, jsonDados, url);
            Console.WriteLine("🔄 Resposta recebida:");
            Console.WriteLine(requisicao.Result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erro ao enviar requisição: {ex.Message}");
        }
    }
}
