namespace MigraPatrim.Models.ModelSybase;

public class DepreciacaoCabecalhoBethaDba
{
    public string ano { get; set; }
    public string mes { get; set; }
    public string mes_ano { get; set; }
}

public class DepreciacoesBethaDba
{
    public int i_depreciacao { get; set; }
    public int i_bem { get; set; }
    public string data_depr { get; set; }
    public decimal saldo_ant { get; set; }
    public decimal percentual { get; set; }
    public string nro_portaria { get; set; }
    public string dt_portaria { get; set; }
    public string matricula_pessoal { get; set; }
    public string dt_autorizacao { get; set; }
    public decimal valor_calc { get; set; }
    public int i_reav_bem { get; set; }
    public int i_entidades { get; set; }
}
