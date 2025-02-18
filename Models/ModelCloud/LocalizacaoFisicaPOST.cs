namespace MigraPatrim.Models.ModelCloud;

public class LocalizacaoFisicaPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
    public int nivel { get; set; }
    public ClassificacaoLocalizacaoFisicaPOST classificacao { get; set; }
    public EnderecoLocalizacaoFisicaPOST endereco { get; set; }
    public string observacao { get; set; }
    public LocalizacaoFisicaPaiLocalizacaoFisicaPOST localizacaoFisicaPai { get; set; }
}

public class ClassificacaoLocalizacaoFisicaPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class EnderecoLocalizacaoFisicaPOST
{
    public int id { get; set; }
}

public class LocalizacaoFisicaPaiLocalizacaoFisicaPOST
{
    public int id { get; set; }
}