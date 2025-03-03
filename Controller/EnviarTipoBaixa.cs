using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelCloud;
using MigraPatrim.Models.ModelPostgres;
using MigraPatrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

public class EnviarTipoBaixa
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarTipoBaixa(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/tipos-baixa");
    }

    public async Task<List<TipoBaixa>> BuscarTipoBaixa()
    {
        const string query = "SELECT * FROM public.tipo_baixa_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<TipoBaixa>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar tipo_baixa_cloud: {ex.Message}");
            return new List<TipoBaixa>();
        }
    }

    public async Task<List<string>> EnviarTipoBaixaCloud()
    {
        var tipos = await BuscarTipoBaixa();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new TipoBaixaPOST
            {
                descricao = item.descricao.Trim().ToUpper(),
                classificacao = new ClassificacaoTipoBaixaPOST
                {
                    valor = item.classificacao,
                    descricao = item.classificacao
                }
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar tipo_baixa_cloud {item.id}: {errorMessage}\n");
                }
                else
                {
                    lotesIds.Add(resposta);
                    Console.WriteLine($"Atualizando ID Cloud: {item.id} -> {resposta}");
                    await AtualizarIdCloud(item.id, resposta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar tipo_baixa_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: tipo_baixa_cloud.\n\n");
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
        const string query = "UPDATE tipo_baixa_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para tipo_baixa_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para tipo_baixa_cloud {id}: {ex.Message}");
        }
    }
}
