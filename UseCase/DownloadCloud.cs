using System.Threading.Tasks;
using MigraPatrim.Connections;
using MigraPatrim.DownloadGLB;

namespace MigraPatrim.UseCase;

public class DownloadCloud
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public DownloadCloud(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task Executar()
    {
        var paises = new Paises(_pgConnection, _token);
        await paises.BuscarPaises();
    }
}
