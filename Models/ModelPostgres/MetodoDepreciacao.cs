namespace MigraPatrim.Models.ModelPostgres;

public class MetodoDepreciacao
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public string descricao { get; set; }
    public string tipo { get; set; }
    public string classificacao { get; set; }
    public bool ativo { get; set; }
}
