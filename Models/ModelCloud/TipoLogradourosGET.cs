using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class TipoLogradourosGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentTipoLogradourosGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentTipoLogradourosGET
{
    public int id { get; set; }
    public List<LinkTipoLogradourosGET> links { get; set; }
    public string descricao { get; set; }
    public string abreviatura { get; set; }
}

public class LinkTipoLogradourosGET
{
    public string rel { get; set; }
    public string href { get; set; }
}