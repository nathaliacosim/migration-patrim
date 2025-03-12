namespace migracao-patrim.Models.ModelCloud;

public class TipoBemPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
    public ClassificacaoTipoBemPOST classificacao { get; set; }
}

public class ClassificacaoTipoBemPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}
