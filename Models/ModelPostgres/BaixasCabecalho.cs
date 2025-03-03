namespace MigraPatrim.Models.ModelPostgres;

public class BaixasCabecalho
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int i_motivo { get; set; }
    public int id_cloud_tipo_baixa { get; set; }
    public string mes { get; set; }
    public string ano { get; set; }
    public string mes_ano { get; set; }
    public string dt_baixa { get; set; }
    public string observacao { get; set; }
}
