namespace MigraPatrim.Models.ModelCloud;

public class OrganogramaPOST
{
    public int id { get; set; }
    public ConfiguracaoOrganogramaOrganogramaPOST configuracaoOrganograma { get; set; }
    public string numeroOrganograma { get; set; }
    public string descricao { get; set; }
    public int nivel { get; set; }
}

public class ConfiguracaoOrganogramaOrganogramaPOST
{
    public int id { get; set; }
}