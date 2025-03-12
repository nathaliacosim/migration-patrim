namespace migracao-patrim.Models.ModelCloud;

public class TombarBemPOST
{
    public int id { get; set; }

    public string nroPlaca { get; set; }

    public OrganogramaTombarBemPOST organograma { get; set; }

    public ResponsavelTombarBemPOST responsavel { get; set; }

    public string dhTombamento { get; set; }
}

public class OrganogramaTombarBemPOST
{
    public int id { get; set; }
}

public class ResponsavelTombarBemPOST
{
    public int id { get; set; }
}