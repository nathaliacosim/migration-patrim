using System;
using System.Threading.Tasks;
using MigraPatrim.Connections;
using MigraPatrim.UseCase;

namespace MigraPatrim;

public class Program
{
    public static async Task Main(string[] args)
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

        /* Executando o processo de Configuração */
        //Configuracoes config = new Configuracoes(pgConnection);
        //await config.Executar();

        /* Executando o processo de Download */
        //Download download = new Download(odbcConnection, pgConnection);
        //await download.Executar();

        /* Executando o processo de Envio dos Dados para Patrimonio Cloud CM Nova Andradina */
        //EnvioDados enviar = new EnvioDados(pgConnection, tokenConversao);
        //await enviar.Executar();

        Console.WriteLine("Processo finalizado.");
    }
}
