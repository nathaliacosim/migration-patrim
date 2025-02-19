using MigraPatrim.Connections;
using MigraPatrim.DownloadSybase;
using System.Threading.Tasks;

namespace MigraPatrim.UseCase;

public class Configuracoes
{
    private readonly PgConnect _pgConnection;

    public Configuracoes(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task Executar()
    {
        //MetodoDepreciacao metodoDepreciacao = new MetodoDepreciacao(_pgConnection);
        //await metodoDepreciacao.MontarRegistro();

        TipoBem tipoBem = new TipoBem(_pgConnection);
        await tipoBem.InserirRegistrosPadrao();
    }
}
