using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class ContentResponsavelGET
{
    public int id { get; set; }
    public List<LinkResponsavelGET> links { get; set; }
    public string nome { get; set; }
    public string cpf { get; set; }
    public bool ehResponsavelMunicipio { get; set; }
    public object naturezaCargo { get; set; }
    public object matricula { get; set; }
    public object cargo { get; set; }
    public bool ativo { get; set; }
    public object funcao { get; set; }
    public object complementoFuncao { get; set; }
    public object dataInativacao { get; set; }
    public object endereco { get; set; }
    public object telefone { get; set; }
    public object email { get; set; }
}

public class LinkResponsavelGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class ResponsavelGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentResponsavelGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}