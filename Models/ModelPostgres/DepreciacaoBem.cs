namespace migracao-patrim.Models.ModelPostgres;

public class DepreciacaoBem
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int i_depreciacao { get; set; }
    public int i_bem { get; set; }
    public int id_cloud_bem { get; set; }
    public int id_cloud_depreciacao { get; set; }
    public string data_depreciacao { get; set; }
    public decimal valor_depreciado { get; set; }
    public int i_entidades { get; set; }
}
