using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class PaisesGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentPaisesGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentPaisesGET
{
    public int id { get; set; }
    public List<LinkPaisesGET> links { get; set; }
    public string nome { get; set; }
}

public class LinkPaisesGET
{
    public string rel { get; set; }
    public string href { get; set; }
}