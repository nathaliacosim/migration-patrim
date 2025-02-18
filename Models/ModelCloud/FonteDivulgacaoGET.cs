using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class FonteDivulgacaoGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentFonteDivulgacaoGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentFonteDivulgacaoGET
{
    public int id { get; set; }
    public List<LinkFonteDivulgacaoGET> links { get; set; }
    public string nome { get; set; }
    public MeioComunicacaoFonteDivulgacaoGET meioComunicacao { get; set; }
}

public class LinkFonteDivulgacaoGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class MeioComunicacaoFonteDivulgacaoGET
{
    public string valor { get; set; }
    public string descricao { get; set; }
}