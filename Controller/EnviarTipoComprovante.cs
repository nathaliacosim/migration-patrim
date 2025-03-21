﻿using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Models.ModelCloud;
using migracao-patrim.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.Controller;

public class EnviarTipoComprovante
{
    private readonly PgConnect _pgConnection;
    private readonly PostRequest _postRequest;

    public EnviarTipoComprovante(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _postRequest = new PostRequest(token, "api/tipos-comprovante");
    }

    public async Task<List<Models.ModelPostgres.TipoComprovante>> BuscarTipoComprovante()
    {
        const string query = "SELECT * FROM public.tipo_comprovante_cloud";
        try
        {
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.TipoComprovante>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar tipo_comprovante_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.TipoComprovante>();
        }
    }

    public async Task<List<string>> EnviarTipoComprovanteCloud()
    {
        var tipos = await BuscarTipoComprovante();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var json = JsonConvert.SerializeObject(new TipoComprovantePOST
            {
                descricao = item.descricao,
                tipo = new TipoTipoComprovantePOST
                {
                    valor = item.tipo
                }
            });

            Console.WriteLine($"Enviando dados: {json}\n");

            try
            {
                var resposta = await _postRequest.Send(json).ConfigureAwait(false);
                if (TryParseErrorResponse(resposta, out var errorMessage))
                {
                    Console.WriteLine($"Erro ao enviar tipo_comprovante_cloud {item.id}: {errorMessage}\n");
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
                Console.WriteLine($"Erro ao enviar tipo_comprovante_cloud {item.id}: {ex.Message}\n");
            }
        }

        Console.WriteLine("FINALIZADO: tipo_comprovante_cloud.\n\n");
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
        const string query = "UPDATE tipo_comprovante_cloud SET id_cloud = @idCloud WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"ID Cloud atualizado para tipo_comprovante_cloud {id}: {idCloud}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar ID Cloud para tipo_comprovante_cloud {id}: {ex.Message}");
        }
    }
}
