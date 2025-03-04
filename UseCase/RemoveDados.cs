using MigraPatrim.Connections;
using MigraPatrim.Controller;
using System.Threading.Tasks;

namespace MigraPatrim.UseCase;

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
    }
}
