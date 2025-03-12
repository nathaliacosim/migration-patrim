using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using migracao-patrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.Controller;

public class EnviarCabecalhoDepreciacao
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarCabecalhoDepreciacao(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/depreciacoes");
    }

    public async Task<List<Models.ModelPostgres.DepreciacaoCabecalho>> BuscarCabecalhoDepreciacoes()
    {
        const string query = "SELECT * FROM depreciacao_cabecalho_cloud ORDER BY ano, mes;";
        try
        {
            using var connection = _pgConnection.GetConnection();
            Console.WriteLine("🔍 Buscando cabeçalhos de depreciação...");
            var cabecalhos = (await connection.QueryAsync<Models.ModelPostgres.DepreciacaoCabecalho>(query)).AsList();
            Console.WriteLine($"✅ {cabecalhos.Count} cabeçalhos encontrados!");
            return cabecalhos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar depreciacao_cabecalho_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.DepreciacaoCabecalho>();
        }
    }

    public async Task<List<string>> EnviarCabecalhoDepreciacoesCloud()
    {
        Console.WriteLine("📤 Iniciando envio dos cabeçalhos de depreciação...");
        var resultado = await BuscarCabecalhoDepreciacoes();
        var lotesIds = new List<string>();

        foreach (var item in resultado)
        {
            var json = JsonConvert.SerializeObject(new DepreciacoesPOST
            {
                mesAno = item.mes_ano
            });

            Console.WriteLine($"📡 Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"⚠️ Erro ao enviar depreciacao_cabecalho_cloud {item.id}: {errorMessage}\n");
                }
                else
                {
                    lotesIds.Add(resposta);
                    Console.WriteLine($"🔄 Atualizando ID Cloud: {item.id} -> {resposta}");
                    await AtualizarIdCloud(item.id, resposta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao enviar depreciacao_cabecalho_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("🎉 FINALIZADO: depreciacao_cabecalho_cloud enviado com sucesso! ✅\n\n");
        return lotesIds;
    }

    private static bool TryParseErrorResponse(string resposta, out string message)
    {
        try
        {
            var erro = JsonConvert.DeserializeObject<Dictionary<string, string>>(resposta);
            return erro.TryGetValue("message", out message);
        }
        catch (JsonException)
        {
            message = null;
            return false;
        }
    }

    public async Task AtualizarIdCloud(int id, string idCloud)
    {
        const string query = "UPDATE depreciacao_cabecalho_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"✅ ID Cloud atualizado para depreciacao_cabecalho_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao atualizar ID Cloud para depreciacao_cabecalho_cloud {id}: {ex.Message}");
        }
    }
}