using MigraPatrim.Connections;

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

    public void Executar(string tokenCliente)
    {
        //var montarConfiguracoes = new DownloadSybase.ConfiguracaoOrganograma(_odbcConnection, _pgConnection);
        //var copiarExercicios = new DownloadSybase.Exercicios(_odbcConnection, _pgConnection);
    }
}
