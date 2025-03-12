namespace migracao-patrim.Models.ModelCloud;

public class EspecieBemPOST
{
    public GrupoBemEspecieBemPOST grupoBem { get; set; }
    public string descricao { get; set; }
}

public class GrupoBemEspecieBemPOST
{
    public int id { get; set; }
}