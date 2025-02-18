using MigraPatrim.Connections;

namespace MigraPatrim.UseCase;

public class Configuracoes
{
    private readonly OdbcConnect _odbcConnection;
    private readonly PgConnect _pgConnection;

    public Configuracoes(OdbcConnect odbcConnection, PgConnect pgConnection)
    {
        _odbcConnection = odbcConnection;
        _pgConnection = pgConnection;
    }

    public void Executar(string tokenCliente)
    {
        //var enviarConfigOrganograma = new Controller.EnviarConfiguracaoOrganograma(_pgConnection, tokenCliente);
        var enviarExercicios = new Controller.EnviarExercicios(_pgConnection, tokenCliente);
    }
}
