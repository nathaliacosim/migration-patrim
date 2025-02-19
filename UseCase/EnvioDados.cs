using MigraPatrim.Connections;
using MigraPatrim.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        //var organogramas = new EnviarOrganogramas(_pgConnection, _token);
        //await organogramas.EnviarOrganogramasCloud();

        //var metodosDepreciacao = new EnviarMetodoDepreciacao(_pgConnection, _token);
        //await metodosDepreciacao.EnviarMetodoDepreCloud();

        var tiposBem = new EnviarTipoBem(_pgConnection, _token);
        await tiposBem.EnviarTipoBemCloud();
    }
}
