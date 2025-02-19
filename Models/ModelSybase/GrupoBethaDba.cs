namespace MigraPatrim.Models.ModelSybase;

public class GrupoBethaDba
{
    public int i_conta { get; set; }
    public string descricao { get; set; }
    public string sigla_tipo_conta { get; set; }
    public int sigla_classif_conta { get; set; }
    public string sigla_tipo_bem { get; set; }
    public decimal percentual_depreciacao { get; set; }
    public decimal percentual_residual { get; set; }
}
