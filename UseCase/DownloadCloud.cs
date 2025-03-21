﻿using System.Threading.Tasks;
using migracao-patrim.Connections;
using migracao-patrim.Controller;
using migracao-patrim.DownloadGLB;

namespace migracao-patrim.UseCase;

public class DownloadCloud
{
    private readonly PgConnect _pgConnection;
    private readonly string _token;

    public DownloadCloud(PgConnect pgConnection, string token)
    {
        _pgConnection = pgConnection;
        _token = token;
    }

    public async Task Executar()
    {
        //var paises = new Paises(_pgConnection, _token);
        //await paises.BuscarPaises();

        //var estados = new Estados(_pgConnection, _token);
        //await estados.BuscarEstados();

        //var municipios = new Municipios(_pgConnection, _token);
        //await municipios.BuscarMunicipios();

        //var tipoLogradouros = new TipoLogradouros(_pgConnection, _token);
        //await tipoLogradouros.BuscarTipoLogradouros();

        //var logradouros = new Logradouros(_pgConnection, _token);
        //await logradouros.BuscarLogradouros();

        //var bairros = new Bairros(_pgConnection, _token);
        //await bairros.BuscarBairros();

        //TipoTransferencia tipoTransf = new TipoTransferencia(_pgConnection, _token);
        //await tipoTransf.BuscarTipoTransferencia();

        //BuscarFornecedores buscarFornecedores = new BuscarFornecedores(_pgConnection, _token);
        //await buscarFornecedores.BuscarFornecs();

        //BuscarResponsaveis buscarResponsaveis = new BuscarResponsaveis(_pgConnection, _token);
        //await buscarResponsaveis.BuscarRespons();

        //BuscarOrganogramas buscarOrganogramas = new BuscarOrganogramas(_pgConnection, _token);
        //await buscarOrganogramas.BuscarOrganog();

    }
}
