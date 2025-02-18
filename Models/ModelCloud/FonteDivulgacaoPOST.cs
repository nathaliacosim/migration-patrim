namespace MigraPatrim.Models.ModelCloud;

public class FonteDivulgacaoPOST
{
    public int id { get; set; }
    public string nome { get; set; }
    public MeioComunicacaoFonteDivulgacaoPOST meioComunicacao { get; set; }
}

public class MeioComunicacaoFonteDivulgacaoPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}