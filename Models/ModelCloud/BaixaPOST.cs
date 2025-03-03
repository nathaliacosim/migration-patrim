namespace MigraPatrim.Models.ModelCloud;

public class BaixaPOST
{
    public TipoBaixaBaixaPOST tipoBaixa { get; set; }
    public string dhBaixa { get; set; }
    public string motivo { get; set; }
}

public class TipoBaixaBaixaPOST
{
    public int id { get; set; }
}