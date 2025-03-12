namespace migracao-patrim.Models.ModelSybase;

public class TipoNaturezaBethaDba
{
    public int i_conta { get; set; }
    public int i_chave { get; set; }
    public string tipo_chave { get; set; }
    public string descricao { get; set; }
}

public class TiposNaturezaBethaDba
{
    public int i_tipo_natur { get; set; }
    public string nome { get; set; }
    public int i_entidades { get; set; }
    public string veiculo { get; set; }
}
