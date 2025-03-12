using migracao-patrim.Connections;
using migracao-patrim.Controller;
using migracao-patrim.Models.ModelPostgres;
using System.Threading.Tasks;

namespace migracao-patrim.UseCase;

public class EnvioDados
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public EnvioDados(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task Executar()
    {
        //EnviarOrganogramas organogramas = new EnviarOrganogramas(_pgConnection, _token);
        //await organogramas.EnviarOrganogramasCloud();

        /* Tratamento de dados CM Nova Andradina */
        /* Depara Organogramas*/
        //EnviarOrganogramasDepara enviarOrganogramas = new EnviarOrganogramasDepara(_pgConnection, _token);
        //await enviarOrganogramas.EnviarOrganogramasCloud();

        //EnviarMetodoDepreciacao metodosDepreciacao = new EnviarMetodoDepreciacao(_pgConnection, _token);
        //await metodosDepreciacao.EnviarMetodoDepreCloud();

        //EnviarTipoBem tiposBem = new EnviarTipoBem(_pgConnection, _token);
        //await tiposBem.EnviarTipoBemCloud();

        //EnviarTipoComprovante tipoComprovante = new EnviarTipoComprovante(_pgConnection, _token);
        //await tipoComprovante.EnviarTipoComprovanteCloud();

        //EnviarTipoUtilizacao tipoUtilizacao = new EnviarTipoUtilizacao(_pgConnection, _token);
        //await tipoUtilizacao.EnviarTipoUtilizacaoCloud();

        //EnviarGrupoBem grupoBem = new EnviarGrupoBem(_pgConnection, _token);
        //await grupoBem.EnviarGrupoBemCloud();

        //EnviarEspecieBem especie = new EnviarEspecieBem(_pgConnection, _token);
        //await especie.EnviarEspecieBemCloud();

        //EnviarResponsaveis respons = new EnviarResponsaveis(_pgConnection, _token);
        //await respons.EnviarResponsaveisCloud();

        //EnviarBairroFornecedores bairros = new EnviarBairroFornecedores(_pgConnection, _token);
        //await bairros.EnviarBairros();

        //EnviarLogradouroFornecedores logradouros = new EnviarLogradouroFornecedores(_pgConnection, _token);
        //await logradouros.EnviarLogradouros();

        //EnviarFornecedores fornecedores = new EnviarFornecedores(_pgConnection, _token);
        //await fornecedores.EnviarFornecedoresCloud();

        //EnviarTipoAquisicao tipoAquisicao = new EnviarTipoAquisicao(_pgConnection, _token);
        //await tipoAquisicao.EnviarAquisicaoCloud();

        //EnviarEstadoConservacao estadoConservacao = new EnviarEstadoConservacao(_pgConnection, _token);
        //await estadoConservacao.EnviarEstadoConservacaoCloud();

        //EnviarBens enviarBens = new EnviarBens(_pgConnection, _token);
        //await enviarBens.EnviarBensCloud();

        //AguardarTombamento aguardarTombamento = new AguardarTombamento(_pgConnection, _token);
        //await aguardarTombamento.AguardandoTombamento();

        //TombarBens tombarBens = new TombarBens(_pgConnection, _token);
        //await tombarBens.Tombar();

        //EnviarCabecalhoDepreciacao enviarCabecalhoDepreciacao = new EnviarCabecalhoDepreciacao(_pgConnection, _token);
        //await enviarCabecalhoDepreciacao.EnviarCabecalhoDepreciacoesCloud();

        //EnviarCabecalhoTransferencia enviarCabecalhoTransferencia = new EnviarCabecalhoTransferencia(_pgConnection, _token);
        //await enviarCabecalhoTransferencia.EnviarCabecalhoTransferenciaCloud();

        //EnviarTipoBaixa tipoBaixa = new EnviarTipoBaixa(_pgConnection, _token);
        //await tipoBaixa.EnviarTipoBaixaCloud();

        //EnviarCabecalhoBaixa enviarCabecalhoBaixa = new EnviarCabecalhoBaixa(_pgConnection, _token);
        //await enviarCabecalhoBaixa.EnviarCabecalhoBaixaCloud();

        EnviarMovimentos enviarMovimentos = new EnviarMovimentos(_pgConnection, _token);
        await enviarMovimentos.ProcessarPacotes();
    }
}
