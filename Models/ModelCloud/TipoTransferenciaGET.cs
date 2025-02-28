using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class TipoTransferenciaGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentTipoTransferenciaGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ClassificacaoTipoTransferenciaGET
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class ContentTipoTransferenciaGET
{
    public int id { get; set; }
    public List<LinkTipoTransferenciaGET> links { get; set; }
    public string descricao { get; set; }
    public ClassificacaoTipoTransferenciaGET classificacao { get; set; }
}

public class LinkTipoTransferenciaGET
{
    public string rel { get; set; }
    public string href { get; set; }
}