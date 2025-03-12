namespace migracao-patrim.Models.ModelSybase;

public class OrgaoBethaDba
{
    public string orgao { get; set; }
    public string descricao { get; set; }
    public int nivel { get; set; }
}

public class UnidadeBethaDba
{
    public string orgao { get; set; }
    public string unidade { get; set; }
    public string descricao { get; set; }
    public int nivel { get; set; }
}

public class CentroCustoBethaDba
{
    public string orgao { get; set; }
    public string unidade { get; set; }
    public string centro_custo { get; set; }
    public string descricao { get; set; }
    public int nivel { get; set; }
}