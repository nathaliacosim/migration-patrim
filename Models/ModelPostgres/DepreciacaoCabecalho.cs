namespace migracao-patrim.Models.ModelPostgres;

public class DepreciacaoCabecalho
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public string mes { get; set; }
    public string ano { get; set; }
    public string mes_ano { get; set; }
    public string observacao { get; set; }
}
