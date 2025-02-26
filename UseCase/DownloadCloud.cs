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
        //var paises = new Paises(_pgConnection, _token);
        //await paises.BuscarPaises();

        //var estados = new Estados(_pgConnection, _token);
        //await estados.BuscarEstados();

        //var municipios = new Municipios(_pgConnection, _token);
        //await municipios.BuscarMunicipios();

        //var tipoLogradouros = new TipoLogradouros(_pgConnection, _token);
        //await tipoLogradouros.BuscarTipoLogradouros();

        //var logradouros = new Logradouros(_pgConnection, _token);
        //await logradouros.BuscarLogradouros();

        var bairros = new Bairros(_pgConnection, _token);
        await bairros.BuscarBairros();
    }
}
