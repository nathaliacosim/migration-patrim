namespace MigraPatrim.Models.ModelPostgres;

public class EspecieBem
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int id_grupo_bem { get; set; }
    public int i_conta { get; set; }
    public int i_chave { get; set; }
    public string tipo_chave { get; set; }
    public string descricao { get; set; }
}
