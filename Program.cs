using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MigraPatrim.Connections;
using MigraPatrim.UseCase;

namespace MigraPatrim;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 🔹 Carregar configurações do appsettings.json
        var config = LoadConfiguration();
        string tokenConversao = config["TokenConversao"];

        Console.WriteLine($"Token de conversão: {tokenConversao}");

        // 🔹 Configurar conexões
        var odbcConnection = ConfigureOdbc(config);
        var pgConnection = ConfigurePostgres(config);

        // 🔹 Executar processo de Download Cloud
        await new DownloadCloud(pgConnection, tokenConversao).Executar();

        Console.WriteLine("Processo finalizado.");
    }

    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private static OdbcConnect ConfigureOdbc(IConfiguration config)
    {
        string dsn = config["ODBC:DSN"];
        Console.WriteLine($"Iniciando conexão ODBC ao DNS: {dsn}");

        var connection = new OdbcConnect(dsn);
        connection.Connect();
        return connection;
    }

    private static PgConnect ConfigurePostgres(IConfiguration config)
    {
        string host = config["Postgres:Host"];
        int port = int.Parse(config["Postgres:Port"]);
        string database = config["Postgres:Database"];
        string username = config["Postgres:Username"];
        string password = config["Postgres:Password"];

        Console.WriteLine($"Iniciando conexão Postgres ao DB: {database}");

        var connection = new PgConnect(host, port, database, username, password);
        connection.Connect();
        return connection;
    }
}
