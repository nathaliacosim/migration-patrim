using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class MunicipiosGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentMunicipiosGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentMunicipiosGET
{
    public int id { get; set; }
    public List<LinkMunicipiosGET> links { get; set; }
    public EstadoMunicipiosGET estado { get; set; }
    public string nome { get; set; }
    public int? codigoIbge { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
    public int? codigoTribunal { get; set; }
    public int? codigoSiafi { get; set; }
}

public class EstadoMunicipiosGET
{
    public int id { get; set; }
}

public class LinkMunicipiosGET
{
    public string rel { get; set; }
    public string href { get; set; }
}
