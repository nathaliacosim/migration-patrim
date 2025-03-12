namespace migracao-patrim.Models.ModelPostgres;

public class BaixaBemEstorno
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int i_estorno { get; set; }
    public int i_baixa { get; set; }
    public int i_motivo { get; set; }
    public int i_bem { get; set; }
    public int id_cloud_bem { get; set; }
    public int id_cloud_baixa { get; set; }
    public string data_estorno { get; set; }
    public string nota_explicativa { get; set; }
    public int i_entidades { get; set; }
}