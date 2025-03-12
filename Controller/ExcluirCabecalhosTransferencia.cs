using Dapper;
using migracao-patrim.Connections;
using migracao-patrim.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace migracao-patrim.Controller;

public class ExcluirCabecalhosTransferencia
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public ExcluirCabecalhosTransferencia(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task<List<Models.ModelPostgres.TransferenciaCabecalho>> BuscarCabecalhoTR()
    {
        const string query = "SELECT * FROM public.transferencia_cabecalho_cloud;";
        try
        {
            Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine(" 🔍  INICIANDO BUSCA DE CABEÇALHOS DE TRANSFERÊNCIAS...");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            using var connection = _pgConnection.GetConnection();
            var result = (await connection.QueryAsync<Models.ModelPostgres.TransferenciaCabecalho>(query)).AsList();

            Console.WriteLine($" ✅  {result.Count} registros encontrados na tabela transferencia_cabecalho_cloud.");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌  ERRO AO BUSCAR OS CABEÇALHOS DE TRANSFERÊNCIAS!");
            Console.WriteLine($"   → {ex.Message}");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            return new List<Models.ModelPostgres.TransferenciaCabecalho>();
        }
    }

    public async Task<List<string>> ExcluirCabecalhoTR()
    {
        Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine(" 🗑️  INICIANDO PROCESSO DE EXCLUSÃO...");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        var tipos = await BuscarCabecalhoTR();
        var lotesIds = new List<string>();

        if (tipos.Count == 0)
        {
            Console.WriteLine("⚠️  Nenhum cabeçalho de transferência encontrado para exclusão.");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            return lotesIds;
        }

        foreach (var item in tipos)
        {
            var url_base = $"https://patrimonio.betha.cloud/patrimonio-services/api/transferencia/{item.id_cloud}";
            Console.WriteLine($" 🔹  Excluindo cabeçalho de transferência com ID {item.id_cloud}...");
            Delete(url_base);
        }

        Console.WriteLine("✅  Todos os cabeçalhos de transferências foram excluídos com sucesso!");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        return lotesIds;
    }

    public void Delete(string url)
    {
        try
        {
            var requisicao = RequisicaoHttp.DeleteRequisicao(_token, url);
            Console.WriteLine($" 🗑️  Requisição DELETE enviada para: {url}");
            Console.WriteLine($" 📩  Resposta: {requisicao.Result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌  ERRO AO EXCLUIR REGISTRO!");
            Console.WriteLine($"   → {ex.Message}");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }
    }
}
