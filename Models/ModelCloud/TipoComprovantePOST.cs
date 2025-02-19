namespace MigraPatrim.Models.ModelCloud;

public class TipoComprovantePOST
{
    public int id { get; set; }
    public string descricao { get; set; }
    public TipoTipoComprovantePOST tipo { get; set; }
}

public class TipoTipoComprovantePOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}
