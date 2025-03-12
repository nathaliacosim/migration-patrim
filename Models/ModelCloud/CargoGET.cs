using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class CargoGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentCargoGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentCargoGET
{
    public int id { get; set; }
    public List<LinkCargoGET> links { get; set; }
    public string nome { get; set; }
}

public class LinkCargoGET
{
    public string rel { get; set; }
    public string href { get; set; }
}