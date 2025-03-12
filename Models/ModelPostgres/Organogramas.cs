namespace migracao-patrim.Models.ModelPostgres;

public class Organogramas
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int id_configuracao { get; set; }
    public string descricao { get; set; }
    public string numero { get; set; }
    public int nivel { get; set; }
}

public class OrganogramaDePara
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int id_configuracao_cloud { get; set; }
    public string numero_organograma { get; set; }
    public string descricao_organograma { get; set; }
    public string orgao_numero { get; set; }
    public string unidade_numero { get; set; }
    public string centro_custo_numero { get; set; }
    public string antigos { get; set; }
}