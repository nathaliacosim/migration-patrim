using Dapper;
using MigraPatrim.Connections;
using MigraPatrim.Models.ModelPostgres;
using MigraPatrim.Models.ModelSybase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MigraPatrim.DownloadSybase
{
    public class Responsavel
    {
        private readonly OdbcConnect _odbcConnection;
        private readonly PgConnect _pgConnection;

        public Responsavel(OdbcConnect odbcConnection, PgConnect pgConnection)
        {
            _odbcConnection = odbcConnection;
            _pgConnection = pgConnection;
        }

        public async Task<List<ResponsavelBethaDba>> BuscarResponsaveis()
        {
            const string query = "SELECT DISTINCT r.i_respons, r.nome, r.cpf, r.funcao " +
                                 "FROM bethadba.bens b " +
                                 "JOIN bethadba.responsaveis r "+
                                 "ON b.i_respons = r.i_respons "+
                                 "WHERE b.i_respons is not null " +
                                 "ORDER BY r.i_respons";
            try
            {
                using var connection = _odbcConnection.GetConnection();
                return (await connection.QueryAsync<ResponsavelBethaDba>(query)).AsList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar os dados: {ex.Message}");
                return new List<ResponsavelBethaDba>();
            }
        }

        public async Task InsertIntoResponsaveis()
        {
            var dadosList = await BuscarResponsaveis();

            foreach (var item in dadosList)
            {
                var parametros = CriarParametros(item);

                if (await RegistroExiste(parametros.Nome))
                {
                    Console.WriteLine("Registro já existente.");
                    continue;
                }

                await InserirRegistro(parametros);
            }
        }

        private async Task<bool> RegistroExiste(string nome)
        {
            const string query = "SELECT COUNT(1) FROM responsaveis_cloud WHERE nome = @nome";

            using var connection = _pgConnection.GetConnection();
            return await connection.ExecuteScalarAsync<int>(query, new { nome }) > 0;
        }

        private async Task InserirRegistro(object parametros)
        {
            const string insertQuery = @"
                INSERT INTO responsaveis_cloud (id_cloud, i_responsavel, nome, cpf, funcao, vinculado, observacoes) 
                VALUES (@IdCloud, @IResponsavel, @Nome, @Cpf, @Funcao, @Vinculado, @Observacoes)";

            try
            {
                using var connection = _pgConnection.GetConnection();
                await connection.ExecuteAsync(insertQuery, parametros);
                Console.WriteLine("Registro inserido com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir registro: {ex.Message}");
            }
        }

        private dynamic CriarParametros(ResponsavelBethaDba item)
        {
            // IDs específicos com valores fixos
            if (new[] { 14, 16, 19, 26 }.Contains(item.i_respons))
            {
                return new
                {
                    IdCloud = "",
                    IResponsavel = 14,
                    Nome = "SUELEN STEFANINI DE SOUZA SILVA",
                    Cpf = "64550945060",
                    Funcao = "RECEPCIONISTA",
                    Vinculado = "16, 19, 26",
                    Observacoes = "CPF gerado em 4DEVS"
                };
            }

            return new
            {
                IdCloud = "",
                IResponsavel = item.i_respons,
                Nome = item.nome.ToUpper(),
                Cpf = item.cpf,
                Funcao = item.funcao == null ? "" : item.funcao.ToUpper(),
                Vinculado = item.i_respons == 21 ? "46" : "",
                Observacoes = new[] { 60, 83, 28, 24 }.Contains(item.i_respons) ? "CPF gerado em 4DEVS" : ""
            };
        }
    }
}
