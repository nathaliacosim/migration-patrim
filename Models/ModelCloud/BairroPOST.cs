namespace migracao-patrim.Models.ModelCloud;

public class BairroPOST
{
    public MunicipioBairroPOST municipio { get; set; }
    public string nome { get; set; }
}

public class MunicipioBairroPOST
{
    public int id { get; set; }
}