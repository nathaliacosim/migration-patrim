using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.Controller;

public class FinalizarBaixa
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public FinalizarBaixa(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task<List<Models.ModelPostgres.BaixasCabecalho>> ObterBaixasPendentes()
    {
        const string query = "SELECT * FROM baixa_cabecalho_cloud WHERE CAST(LEFT(dt_baixa, 4) AS integer) = 2010 AND finalizado = 'N';";
        try
        {
            using var connection = _pgConnection.GetConnection();
            var baixas = (await connection.QueryAsync<Models.ModelPostgres.BaixasCabecalho>(query)).AsList();
            Console.WriteLine($"🔍 Encontradas {baixas.Count} baixas pendentes para finalizar.");
            return baixas;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao buscar baixas pendentes: {ex.Message}");
            return new List<Models.ModelPostgres.BaixasCabecalho>();
        }
    }

    public async Task<List<string>> FinalizarBaixasPendentes()
    {
        var dados = await ObterBaixasPendentes();
        var lotesIds = new List<string>();

        foreach (var item in dados)
        {
            var url_base = $"https://patrimonio.betha.cloud/patrimonio-services/api/baixas/{item.id_cloud}/finalizada";

            var idCloud = await EnviarFinalizacaoEObterId(url_base);
            if (!string.IsNullOrEmpty(idCloud))
            {
                Console.WriteLine($"✅ Baixa finalizada com sucesso! ID Cloud: {idCloud}");
                await AtualizarStatusDeFinalizacao(item.id, idCloud);
                lotesIds.Add(idCloud);
            }
        }

        Console.WriteLine("🎉 Todo o processo de finalização foi concluído com sucesso!");
        return lotesIds;
    }

    public async Task<string> EnviarFinalizacaoEObterId(string url)
    {
        try
        {
            Console.WriteLine("🌐 Enviando dados para finalizar a baixa...");
            var resposta = await RequisicaoHttp.PostRequisicao(_token, null, url).ConfigureAwait(false);
            return resposta?.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao enviar dados para o Cloud: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task AtualizarStatusDeFinalizacao(int id, string idCloud)
    {
        const string query = "UPDATE baixa_cabecalho_cloud SET id_cloud_finalizacao = @idCloud, finalizado = 'S' WHERE id = @id";

        try
        {
            await _pgConnection.ExecuteAsync(query, new { id, idCloud });
            Console.WriteLine($"🔄 ID Cloud {idCloud} atualizado para a baixa {id}. Status: Finalizado ✔️");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao atualizar ID Cloud para a baixa {id}: {ex.Message}");
        }
    }
}
