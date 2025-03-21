﻿using migracao-patrim.Connections;
using migracao-patrim.DownloadSybase;
using System.Threading.Tasks;

namespace migracao-patrim.UseCase;

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

        //DetalhamentoBem detalhamentoBem = new DetalhamentoBem(_odbcConnection, _pgConnection);
        //await detalhamentoBem.InsertIntoDetalhamentos();

        //TipoNatureza tipoNatureza = new TipoNatureza(_odbcConnection, _pgConnection);
        //await tipoNatureza.InsertIntoTiposNatureza();

        //GrupoBem grupoBem = new GrupoBem(_odbcConnection, _pgConnection);
        //await grupoBem.InserirGrupos();

        //EspecieBem especieBem = new EspecieBem(_odbcConnection, _pgConnection);
        //await especieBem.InsertIntoEspecies();
        //await especieBem.InsertIntoOutrasEspecies();

        //Responsavel resp = new Responsavel(_odbcConnection, _pgConnection);
        //await resp.InsertIntoResponsaveis();

        //Fornecedor fornec = new Fornecedor(_odbcConnection, _pgConnection);
        //await fornec.InsertIntoFornecedor();

        //Enderecos ends = new Enderecos(_odbcConnection, _pgConnection);
        //await ends.InsertIntoEndereco();

        //Localizacao localizacao = new Localizacao(_odbcConnection, _pgConnection);
        //await localizacao.InsertIntoLocalizacaoBem();

        //CadastroBem cadastroBem = new CadastroBem(_odbcConnection, _pgConnection);
        //await cadastroBem.InsertIntoBens();

        //Contas contas = new Contas(_odbcConnection, _pgConnection);
        //await contas.InsertIntoContas();

        //CentroCusto centroCusto = new CentroCusto(_odbcConnection, _pgConnection);
        //await centroCusto.InsertIntoCentroCusto();

        //DepreciacaoCabecalho depreciacaoCabecalho = new DepreciacaoCabecalho(_odbcConnection, _pgConnection);
        //await depreciacaoCabecalho.InsertIntoCabecalhoDepre();

        //TransferenciaCabecalho transfCabecalho = new TransferenciaCabecalho(_odbcConnection, _pgConnection);
        //await transfCabecalho.InsertIntoCabecalhoTransf();

        //TipoBaixa tipoBaixa = new TipoBaixa(_odbcConnection, _pgConnection);
        //await tipoBaixa.InsertIntoTipoBaixa();

        //BaixaCabecalho baixaCabecalho = new BaixaCabecalho(_odbcConnection, _pgConnection);
        //await baixaCabecalho.InsertIntoCabecalhoBaixa();

        //DepreciacaoBens depreciacaoBens = new DepreciacaoBens(_odbcConnection, _pgConnection);
        //await depreciacaoBens.InsertIntoBensDepreciacao();

        //TransferenciaBens transferenciaBens = new TransferenciaBens(_odbcConnection, _pgConnection);
        //await transferenciaBens.InsertIntoTransferenciaBens();

        //BaixaBens baixaBens = new BaixaBens(_odbcConnection, _pgConnection);
        //await baixaBens.InsertIntoBaixaBens();

        //BaixaEstornos baixaEstornos = new BaixaEstornos(_odbcConnection, _pgConnection);
        //await baixaEstornos.InsertIntoEstornoBaixaBens();
    }
}
