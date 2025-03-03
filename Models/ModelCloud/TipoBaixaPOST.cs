namespace MigraPatrim.Models.ModelCloud;

public class TipoBaixaPOST
{
    public string descricao { get; set; }
    public ClassificacaoTipoBaixaPOST classificacao { get; set; }
}

public class ClassificacaoTipoBaixaPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}