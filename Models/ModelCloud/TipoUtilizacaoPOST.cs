namespace migracao-patrim.Models.ModelCloud;

public class TipoUtilizacaoPOST
{
    public string descricao { get; set; }
    public ClassificacaoTipoUtilizacaoPOST classificacao { get; set; }
}

public class ClassificacaoTipoUtilizacaoPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}