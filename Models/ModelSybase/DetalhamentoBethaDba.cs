namespace MigraPatrim.Models.ModelSybase;

public class DetalhamentoBethaDba
{
    public int i_conta { get; set; }
    public int i_chave { get; set; }
    public string tipo_chave { get; set; }
    public string descricao { get; set; }
}

public class DetalhamentosBethaDba
{
    public int i_entidades { get; set; }
    public int i_detalhamentos_bens { get; set; }
    public string descricao { get; set; }
    public string codigo_tce { get; set; }
    public string categoria { get; set; }
    public string natureza { get; set; }
    public string padrao { get; set; }
    public string utilizacao { get; set; }
}
