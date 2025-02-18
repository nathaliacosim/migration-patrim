namespace MigraPatrim.Models.ModelCloud;

public class GrupoBemPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
    public TipoBemGrupoBemPOST tipoBem { get; set; }
    public MetodoDepreciacaoGrupoBemPOST metodoDepreciacao { get; set; }
    public int percentualDepreciacaoAnual { get; set; }
    public int percentualValorResidual { get; set; }
    public int vidaUtil { get; set; }
}

public class MetodoDepreciacaoGrupoBemPOST
{
    public int id { get; set; }
}

public class TipoBemGrupoBemPOST
{
    public int id { get; set; }
}