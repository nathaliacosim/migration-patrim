using System;
using MigraPatrim.Connections;
using MigraPatrim.UseCase;

namespace MigraPatrim;

public class Program
{
    public static void Main(string[] args)
    {
        var tokenConversao = "5dcd336d-a577-4062-af6c-8d524ebedde3";
        Console.WriteLine($"Token de conversão: {tokenConversao}");

        /* Configurando conexão ao Sybase */
        string dsn = "PATRIM_CMNA";
        Console.WriteLine($"Iniciando conexão ODBC ao dns: {dsn}");
        var odbcConnection = new OdbcConnect(dsn);
        odbcConnection.Connect();

        /* Configurando conexão ao Postgres */
        string host = "localhost";
        int port = 5432;
        string database = "CM_NA_PATRIM";
        string username = "postgres";
        string password = "root";
        Console.WriteLine($"Iniciando conexão Postgres ao DB: {database}");
        var pgConnection = new PgConnect(host, port, database, username, password);
        pgConnection.Connect();


        //Download d = new Download(odbcConnection, pgConnection);
        //d.Executar(tokenConversao);

        Configuracoes c = new Configuracoes(odbcConnection, pgConnection);
        c.Executar(tokenConversao);

        Console.ReadLine();
    }
}
