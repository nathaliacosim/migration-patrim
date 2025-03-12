using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class ConfigOrganogramaGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentConfigOrganogramaGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentConfigOrganogramaGET
{
    public int id { get; set; }
    public List<LinkConfigOrganogramaGET> links { get; set; }
    public string descricao { get; set; }
    public object mascara { get; set; }
    public List<NiveisConfigOrganogramaGET> niveis { get; set; }
}

public class LinkConfigOrganogramaGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class NiveisConfigOrganogramaGET
{
    public int id { get; set; }
    public int nivel { get; set; }
    public string descricao { get; set; }
    public int digitos { get; set; }
    public string separador { get; set; }
}