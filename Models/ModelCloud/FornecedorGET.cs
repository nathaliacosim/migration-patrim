using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class FornecedorGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentFornecedorGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentFornecedorGET
{
    public int id { get; set; }
    public List<LinkFornecedorGET> links { get; set; }
    public string nome { get; set; }
    public string cpfCnpj { get; set; }
    public TipoFornecedorGET tipo { get; set; }
    public SituacaoFornecedorGET situacao { get; set; }
    public string dataInclusao { get; set; }
    public PorteEmpresaFornecedorGET porteEmpresa { get; set; }
    public bool optanteSimples { get; set; }
    public List<object> contasBancarias { get; set; }
    public List<EmailFornecedorGET> emails { get; set; }
    public List<TelefoneFornecedorGET> telefones { get; set; }
}

public class EmailFornecedorGET
{
    public int id { get; set; }
    public string endereco { get; set; }
    public int ordem { get; set; }
    public object descricao { get; set; }
}

public class LinkFornecedorGET
{
    public string rel { get; set; }
    public string href { get; set; }
}

public class PorteEmpresaFornecedorGET
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class SituacaoFornecedorGET
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class TelefoneFornecedorGET
{
    public int id { get; set; }
    public string numero { get; set; }
    public object observacao { get; set; }
    public string tipo { get; set; }
    public int ordem { get; set; }
}

public class TipoFornecedorGET
{
    public string valor { get; set; }
    public string descricao { get; set; }
}