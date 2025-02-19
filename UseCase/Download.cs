using MigraPatrim.Connections;
using MigraPatrim.DownloadSybase;
using System.Threading.Tasks;

namespace MigraPatrim.UseCase;

public class Download
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Download(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public async Task Executar()
    {
        //var montarConfiguracoes = new DownloadSybase.ConfiguracaoOrganograma(_odbcConnection, _pgConnection);
        //var copiarExercicios = new DownloadSybase.Exercicios(_odbcConnection, _pgConnection);

        //Organogramas organogramas = new Organogramas(_odbcConnection, _pgConnection);
        //await organogramas.InsertIntoOrganogramas();

        GrupoBem grupoBem = new GrupoBem(_odbcConnection, _pgConnection);
        await grupoBem.InserirGrupos();
    }
}
