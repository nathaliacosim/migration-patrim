using migracao-patrim.Connections;
using migracao-patrim.Controller;
using System.Threading.Tasks;

namespace migracao-patrim.UseCase;

public class RemoveDados
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public RemoveDados(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task Executar()
    {
        ExcluirCabecalhosTransferencia excluirCabecalhosTransferencia = new ExcluirCabecalhosTransferencia(_pgConnection, _token);
        await excluirCabecalhosTransferencia.ExcluirCabecalhoTR();

        //ExcluirCabecalhoBaixas excluirCabecalhoBaixas = new ExcluirCabecalhoBaixas(_pgConnection, _token);
        //await excluirCabecalhoBaixas.ExcluirCabecalhoBX();
    }
}
