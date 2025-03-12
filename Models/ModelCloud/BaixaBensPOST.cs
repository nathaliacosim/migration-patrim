namespace migracao-patrim.Models.ModelCloud;

public class BaixaBaixaBensPOST
{
    public int id { get; set; }
}

public class BemBaixaBensPOST
{
    public int id { get; set; }
}

public class BaixaBensPOST
{
    public BaixaBaixaBensPOST baixa { get; set; }
    public BemBaixaBensPOST bem { get; set; }
    public string notaExplicativa { get; set; }
}
