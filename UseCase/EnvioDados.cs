﻿using MigraPatrim.Connections;
using MigraPatrim.Controller;
using System.Threading.Tasks;

namespace MigraPatrim.UseCase;

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

        //EnviarMetodoDepreciacao metodosDepreciacao = new EnviarMetodoDepreciacao(_pgConnection, _token);
        //await metodosDepreciacao.EnviarMetodoDepreCloud();

        //EnviarTipoBem tiposBem = new EnviarTipoBem(_pgConnection, _token);
        //await tiposBem.EnviarTipoBemCloud();

        EnviarTipoComprovante tipoComprovante = new EnviarTipoComprovante(_pgConnection, _token);
        await tipoComprovante.EnviarTipoComprovanteCloud();
    }
}
