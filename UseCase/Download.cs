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

        //GrupoBem grupoBem = new GrupoBem(_odbcConnection, _pgConnection);
        //await grupoBem.InserirGrupos();

        //EspecieBem especieBem = new EspecieBem(_odbcConnection, _pgConnection);
        //await especieBem.InsertIntoEspecies();

        //Responsavel resp = new Responsavel(_odbcConnection, _pgConnection);
        //await resp.InsertIntoResponsaveis();

        //Fornecedor fornec = new Fornecedor(_odbcConnection, _pgConnection);
        //await fornec.InsertIntoFornecedor();

        //Enderecos ends = new Enderecos(_odbcConnection, _pgConnection);
        //await ends.InsertIntoEndereco();

        //CadastroBem cadastroBem = new CadastroBem(_odbcConnection, _pgConnection);
        //await cadastroBem.InsertIntoBens();

        //Contas contas = new Contas(_odbcConnection, _pgConnection);
        //await contas.InsertIntoContas();

        //CentroCusto centroCusto = new CentroCusto(_odbcConnection, _pgConnection);
        //await centroCusto.InsertIntoCentroCusto();

        DepreciacaoCabecalho depreciacaoCabecalho = new DepreciacaoCabecalho(_odbcConnection, _pgConnection);
        await depreciacaoCabecalho.InsertIntoCabecalhoDepre();
    }
}
