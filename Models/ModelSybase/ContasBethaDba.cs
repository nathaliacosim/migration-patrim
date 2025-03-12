namespace migracao-patrim.Models.ModelSybase;

public class ContasBethaDba
{
    public int i_conta { get; set; }
    public int i_entidades { get; set; }
    public string classif { get; set; }
    public int codigo_tce { get; set; }
    public string descricao { get; set; }
    public decimal? percent_deprec { get; set; }
    public string proc_aquisicao { get; set; }
    public string tipo { get; set; }
    public string tipo_bem { get; set; }
    public decimal? vlr_resid { get; set; }
}
