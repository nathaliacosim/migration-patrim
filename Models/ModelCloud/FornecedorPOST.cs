using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class FornecedorPOST
{
    public string nome { get; set; }
    public string cpfCnpj { get; set; }
    public TipoFornecedorPOST tipo { get; set; }
    public SituacaoFornecedorPOST situacao { get; set; }
    public string dataInclusao { get; set; }
    public EnderecoFornecedorPOST endereco { get; set; }
    public List<EmailFornecedorPOST> emails { get; set; }
    public List<TelefoneFornecedorPOST> telefones { get; set; }
}

public class BairroFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class EmailFornecedorPOST
{
    public int id { get; set; }
    public string endereco { get; set; }
    public string descricao { get; set; }
    public int ordem { get; set; }
}

public class EnderecoFornecedorPOST
{
    public string descricao { get; set; }
    public MunicipioFornecedorPOST municipio { get; set; }
    public BairroFornecedorPOST bairro { get; set; }
    public LogradouroFornecedorPOST logradouro { get; set; }
    public string numero { get; set; }
    public string cep { get; set; }
}

public class LogradouroFornecedorPOST
{
    public int id { get; set; }
}

public class MunicipioFornecedorPOST
{
    public int id { get; set; }
}

public class SituacaoFornecedorPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class TelefoneFornecedorPOST
{
    public int id { get; set; }
    public string numero { get; set; }
    public string observacao { get; set; }
    public string tipo { get; set; }
    public int ordem { get; set; }
}

public class TipoFornecedorPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}