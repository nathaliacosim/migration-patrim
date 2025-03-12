using System.Threading.Tasks;
using migracao-patrim.Connections;
using migracao-patrim.DownloadSybase;

namespace migracao-patrim.UseCase;

public class Configuracoes
{
    private readonly PgConnect _pgConnection;

    public Configuracoes(PgConnect pgConnection)
    {
        _pgConnection = pgConnection;
    }

    public async Task Executar()
    {
        //MetodoDepreciacao metodoDepreciacao = new MetodoDepreciacao(_pgConnection);
        //await metodoDepreciacao.MontarRegistro();

        //TipoBem tipoBem = new TipoBem(_pgConnection);
        //await tipoBem.InserirRegistrosPadrao();

        //TipoComprovante tipoComprovante = new TipoComprovante(_pgConnection);
        //await tipoComprovante.InserirRegistrosPadrao();

        //TipoUtilizacao tipoUtilizacao = new TipoUtilizacao(_pgConnection);
        //await tipoUtilizacao.InserirRegistrosPadrao();

        //TipoAquisicao tipoAquisicao = new TipoAquisicao(_pgConnection);
        //await tipoAquisicao.InserirTipoAquisicao();

        //EstadoConservacao estadoConservacao = new EstadoConservacao(_pgConnection);
        //await estadoConservacao.InsertIntoEstadoConservacao();
    }
}
