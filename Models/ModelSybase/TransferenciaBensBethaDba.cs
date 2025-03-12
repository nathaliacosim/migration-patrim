namespace migracao-patrim.Models.ModelSybase;

public class TransferenciaBensBethaDba
{
    public int i_localizacao { get; set; }
    public int i_bem { get; set; }
    public int i_entidades { get; set; }
    public string data_transf { get; set; }
    public string tipo { get; set; }
    public string novo_cod1 { get; set; }
    public string novo_cod2 { get; set; }
    public string cod_anterior1 { get; set; }
    public string cod_anterior2 { get; set; }
    public string historico { get; set; }
}
