namespace migracao-patrim.Models.ModelPostgres;

public class TransferenciaBem
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int id_cloud_transferencia { get; set; }
    public int i_localizacao { get; set; }
    public int i_bem { get; set; }
    public int i_entidades { get; set; }
    public int id_cloud_bem { get; set; }
    public string data_movimento { get; set; }
    public string sigla_movimento { get; set; }
    public string codigo_novo1 { get; set; }
    public string codigo_novo2 { get; set; }
    public string codigo_antigo1 { get; set; }
    public string codigo_antigo2 { get; set; }
    public string historico { get; set; }
}
