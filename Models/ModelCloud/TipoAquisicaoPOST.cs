﻿namespace MigraPatrim.Models.ModelCloud;

public class TipoAquisicaoPOST
{
    public string descricao { get; set; }
    public ClassificacaoTipoAquisicaoPOST classificacao { get; set; }
}

public class ClassificacaoTipoAquisicaoPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}