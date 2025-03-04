using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MigraPatrim.Controller;

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
            using var connection = _pgConnection.GetConnection();
            return (await connection.QueryAsync<Models.ModelPostgres.TransferenciaCabecalho>(query)).AsList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar bens_cloud: {ex.Message}");
            return new List<Models.ModelPostgres.TransferenciaCabecalho>();
        }
    }

    public async Task<List<string>> ExcluirCabecalhoTR()
    {
        var tipos = await BuscarCabecalhoTR();
        var lotesIds = new List<string>();

        foreach (var item in tipos)
        {
            var url_base = "https://patrimonio.betha.cloud/patrimonio-services/api/transferencia/" + item.id_cloud;
            Delete(url_base);
        }

        return lotesIds;
    }

    public void Delete(string url)
    {
        try
        {
            var requisicao = RequisicaoHttp.DeleteRequisicao(_token, url);
            Console.WriteLine(requisicao.Result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
