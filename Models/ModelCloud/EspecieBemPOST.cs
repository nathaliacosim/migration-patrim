namespace MigraPatrim.Models.ModelCloud;

public class EspecieBemPOST
{
    public int id { get; set; }
    public GrupoBemEspecieBemPOST grupoBem { get; set; }
    public string descricao { get; set; }
}

public class GrupoBemEspecieBemPOST
{
    public int id { get; set; }
}