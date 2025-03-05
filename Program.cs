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
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var isConfig = false;
        var isDownload = false;
        var isGetCloud = false;
        var isEnvio = true;
        var isRemocao = false;

        var config = LoadConfiguration();
        string tokenConversao = config["TokenConversao"];

        // 🔹 Configurar conexões
        var odbcConnection = ConfigureOdbc(config);
        var pgConnection = ConfigurePostgres(config);

        // 🔹 Executar processos
        if (isConfig)
        {
            Console.WriteLine("🔧 Configurando dados fixos... 🔄");
            await new Configuracoes(pgConnection).Executar();
        }

        if (isDownload)
        {
            Console.WriteLine("⬇️ Buscando dados do Sybase e enviando para o PostgreSQL... 📥");
            await new Download(odbcConnection, pgConnection).Executar();
        }

        if (isGetCloud)
        {
            Console.WriteLine("☁️ Buscando dados do Cloud Patrimônio... 🔍");
            await new DownloadCloud(pgConnection, tokenConversao).Executar();
        }

        if (isEnvio)
        {
            Console.WriteLine("📤 Enviando dados para o Cloud Patrimônio... 🚀");
            await new EnvioDados(pgConnection, tokenConversao).Executar();
        }

        if (isRemocao)
        {
            Console.WriteLine("🗑️ Removendo dados do Cloud Patrimônio... ❌");
            await new RemoveDados(pgConnection, tokenConversao).Executar();
        }

        Console.WriteLine("✅ Processo finalizado com sucesso!");
    }

    private static IConfiguration LoadConfiguration()
    {
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        Console.WriteLine($"🌍 Host: {config["Postgres:Host"]}");
        Console.WriteLine($"📍 Porta: {config["Postgres:Port"]}");
        Console.WriteLine($"📚 Banco de Dados: {config["Postgres:Database"]}");
        Console.WriteLine($"🔑 Usuário: {config["Postgres:Username"]}");

        return config;
    }

    private static OdbcConnect ConfigureOdbc(IConfiguration config)
    {
        string dsn = config["ODBC:DSN"];
        Console.WriteLine($"🔌 Iniciando conexão ODBC ao DNS: {dsn}... ⏳");

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

        Console.WriteLine($"🔑 Iniciando conexão Postgres ao DB: {database}... 🔄");

        var connection = new PgConnect(host, port, database, username, password);
        connection.Connect();
        return connection;
    }
}
