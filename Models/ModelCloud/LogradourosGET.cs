using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class LogradourosGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentLogradourosGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class TipoLogradouroLogradourosGET
{
    public int? id { get; set; }
}

public class ContentLogradourosGET
{
    public int? id { get; set; }
    public List<LinkLogradourosGET> links { get; set; }
    public MunicipioLogradourosGET municipio { get; set; }
    public TipoLogradouroLogradourosGET tipoLogradouro { get; set; }
    public string nome { get; set; }
    public string cep { get; set; }
}

public class LinkLogradourosGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class MunicipioLogradourosGET
{
    public int? id { get; set; }
}