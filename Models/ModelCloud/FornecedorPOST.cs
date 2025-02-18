using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class FornecedorPOST
{
    public int id { get; set; }
    public string nome { get; set; }
    public string cpfCnpj { get; set; }
    public TipoFornecedorPOST tipo { get; set; }
    public string nomeFantasia { get; set; }
    public string inscricaoEstadual { get; set; }
    public EstadoInscricaoFornecedorPOST estadoInscricao { get; set; }
    public string inscricaoMunicipal { get; set; }
    public MunicipioInscricaoFornecedorPOST municipioInscricao { get; set; }
    public SituacaoFornecedorPOST situacao { get; set; }
    public string dataSituacao { get; set; }
    public string dataInclusao { get; set; }
    public ResponsavelFornecedorPOST responsavel { get; set; }
    public OrgaoRegistroEmpresaFornecedorPOST orgaoRegistroEmpresa { get; set; }
    public string dataRegistro { get; set; }
    public string numeroRegistro { get; set; }
    public PorteEmpresaFornecedorPOST porteEmpresa { get; set; }
    public bool optanteSimples { get; set; }
    public NaturezaJuridicaFornecedorPOST naturezaJuridica { get; set; }
    public NaturezaJuridicaQualificacaoFornecedorPOST naturezaJuridicaQualificacao { get; set; }
    public EnderecoFornecedorPOST endereco { get; set; }
    public TelefoneFornecedorPOST telefone { get; set; }
    public EmailFornecedorPOST email { get; set; }
    public IdentificadorDesktopTransparenciaFornecedorPOST identificadorDesktopTransparencia { get; set; }
    public List<ContasBancariaFornecedorPOST> contasBancarias { get; set; }
    public List<EmailFornecedorPOST> emails { get; set; }
    public List<TelefoneFornecedorPOST> telefones { get; set; }
}

public class AgenciaBancariaFornecedorPOST
{
    public int id { get; set; }
}

public class BairroFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class CondominioFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class ContasBancariaFornecedorPOST
{
    public int id { get; set; }
    public AgenciaBancariaFornecedorPOST agenciaBancaria { get; set; }
    public string numero { get; set; }
    public string digito { get; set; }
    public string tipoConta { get; set; }
    public string dataAbertura { get; set; }
    public string dataFechamento { get; set; }
    public string situacao { get; set; }
    public bool padrao { get; set; }
}

public class DistritoFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class EmailFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class EnderecoFornecedorPOST
{
    public int id { get; set; }
    public MunicipioFornecedorPOST municipio { get; set; }
    public PessoaFornecedorPOST pessoa { get; set; }
    public DistritoFornecedorPOST distrito { get; set; }
    public BairroFornecedorPOST bairro { get; set; }
    public LogradouroFornecedorPOST logradouro { get; set; }
    public LoteamentoFornecedorPOST loteamento { get; set; }
    public CondominioFornecedorPOST condominio { get; set; }
    public string apartamento { get; set; }
    public string numero { get; set; }
    public string complemento { get; set; }
    public string bloco { get; set; }
    public string cep { get; set; }
    public int ordem { get; set; }
    public string descricao { get; set; }
}

public class EstadoInscricaoFornecedorPOST
{
    public int id { get; set; }
}

public class IdentificadorDesktopTransparenciaFornecedorPOST
{
    public int id { get; set; }
    public int entidadeDesktop { get; set; }
    public string tabela { get; set; }
    public int identificadorPrimeiro { get; set; }
    public int identificadorSegundo { get; set; }
    public int identificadorTerceiro { get; set; }
}

public class LogradouroFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class LoteamentoFornecedorPOST
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class MunicipioFornecedorPOST
{
    public int id { get; set; }
}

public class MunicipioInscricaoFornecedorPOST
{
    public int id { get; set; }
}

public class NaturezaJuridicaFornecedorPOST
{
    public int id { get; set; }
}

public class NaturezaJuridicaQualificacaoFornecedorPOST
{
    public int id { get; set; }
}

public class OrgaoRegistroEmpresaFornecedorPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class PessoaFornecedorPOST
{
    public int id { get; set; }
}

public class PorteEmpresaFornecedorPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class ResponsavelFornecedorPOST
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
    public string descricao { get; set; }
}

public class TipoFornecedorPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}