using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarBaixaBens
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public EnviarBaixaBens(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task<List<Models.ModelPostgres.BaixaBem>> BuscarBensBaixados()
    {
        const string query = @"SELECT * FROM baixas_cloud 
                               WHERE CAST(LEFT(data_baixa, 4) AS integer) != 2010
                               ORDER BY data_baixa";
        try
        {
            using var connection = _pgConnection.GetConnection();
            var result = await connection.QueryAsync<Models.ModelPostgres.BaixaBem>(query);
            Console.WriteLine("📑 Dados de baixas encontrados com sucesso!");
            return result.AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar baixas_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.BaixaBem>();
        }
    }

    public async Task<List<string>> BaixarBem()
    {
        var dados = await BuscarBensBaixados();
        var lotesIds = new List<string>();

        foreach (var item in dados)
        {
            var url_base = "https://patrimonio.betha.cloud/patrimonio-services/api/baixas/" + item.id_cloud_baixa + "/bens";

            var json_dados = new BaixaBensPOST
            {
                notaExplicativa = item?.nota_explicativa?.Trim().ToUpper() ?? "",
                baixa = new BaixaBaixaBensPOST
                {
                    id = item.id_cloud_baixa
                },
                bem = new BemBaixaBensPOST
                {
                    id = item.id_cloud_bem
                }
            };

            var json = JsonConvert.SerializeObject(json_dados);
            Console.WriteLine("🔄 Enviando dados para a API do Cloud...");

            var idCloud = await SendAndGetId(json, url_base);

            if (!string.IsNullOrEmpty(idCloud))
            {
                Console.WriteLine($"✅ ID Cloud retornado: {idCloud}");
                await AtualizarIdCloud(item.id, idCloud);
                lotesIds.Add(idCloud);
            }
        }

        Console.WriteLine("🎉 Bens baixados com sucesso! Processo concluído!");
        return lotesIds;
    }

    public async Task<string> SendAndGetId(string jsonDados, string url)
    {
        try
        {
            Console.WriteLine("🌐 Enviando dados...");
            var resposta = await RequisicaoHttp.PostRequisicao(_token, jsonDados, url).ConfigureAwait(false);
            return resposta?.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar dados para o Cloud: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task AtualizarIdCloud(int id, string idCloud)
    {
        const string query = "UPDATE baixas_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"🔄 ID Cloud atualizado para baixas_cloud {id}: {idCloud} ✔️");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao atualizar ID Cloud para baixas_cloud {id}: {ex.Message}");
        }
    }
}
