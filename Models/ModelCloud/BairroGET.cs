using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class ContentBairroGET
{
    public int id { get; set; }
    public List<LinkBairroGET> links { get; set; }
    public MunicipioBairroGET municipio { get; set; }
    public string nome { get; set; }
}

public class LinkBairroGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class MunicipioBairroGET
{
    public int id { get; set; }
}

public class BairroGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentBairroGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}