using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class EstadosGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentEstadosGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentEstadosGET
{
    public int id { get; set; }
    public List<LinkEstadosGET> links { get; set; }
    public PaisEstadosGET pais { get; set; }
    public string nome { get; set; }
    public string uf { get; set; }
    public int codigoIbge { get; set; }
}

public class LinkEstadosGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class PaisEstadosGET
{
    public int id { get; set; }
}