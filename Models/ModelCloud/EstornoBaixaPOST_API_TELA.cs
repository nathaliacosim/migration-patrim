namespace MigraPatrim.Models.ModelCloud;

public class EstornoBaixaPOST_API_TELA
{
    public BaixaEstornoBaixaPOST_API_TELA baixa { get; set; }
    public string dataEstorno { get; set; }
    public ResponsavelEstornoBaixaPOST_API_TELA responsavel { get; set; }
    public string motivo { get; set; }
}

public class BaixaEstornoBaixaPOST_API_TELA
{
    public int id { get; set; }
}

public class ResponsavelEstornoBaixaPOST_API_TELA
{
    public int id { get; set; }
}
