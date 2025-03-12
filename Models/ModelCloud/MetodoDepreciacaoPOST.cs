namespace migracao-patrim.Models.ModelCloud;

public class MetodoDepreciacaoPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
    public ClassificacaoMetodoDepreciacaoPOST classificacao { get; set; }
    public TipoDepreciacaoMetodoDepreciacaoPOST tipoDepreciacao { get; set; }
    public bool ativo { get; set; }
}

public class ClassificacaoMetodoDepreciacaoPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class TipoDepreciacaoMetodoDepreciacaoPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}
