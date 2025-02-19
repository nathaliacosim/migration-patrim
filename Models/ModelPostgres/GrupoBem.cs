namespace MigraPatrim.Models.ModelPostgres;

public class GrupoBem
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int i_conta { get; set; }
    public string descricao { get; set; }
    public int id_tipo_bem { get; set; }
    public int id_metodo_depreciacao { get; set; }
    public decimal percentual_depreciacao { get; set; }
    public decimal percentual_residual { get; set; }
    public int vida_util { get; set; }
    public string sigla_tipo_bem { get; set; }
    public string sigla_tipo_conta { get; set; }
    public int sigla_classif_conta { get; set; }
}
