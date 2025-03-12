using System.Collections.Generic;

namespace migracao-patrim.Models.ModelCloud;

public class BensGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentBensGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class BemValorBensGET
{
    public int? id { get; set; }
    public MetodoDepreciacaoBensGET metodoDepreciacao { get; set; }
    public MoedaBensGET moeda { get; set; }
    public decimal? vlAquisicao { get; set; }
    public decimal? vlAquisicaoConvertido { get; set; }
    public decimal? vlResidual { get; set; }
    public decimal? vlDepreciavel { get; set; }
    public decimal? vlDepreciado { get; set; }
    public decimal? saldoDepreciar { get; set; }
    public decimal? capacidadeProdutiva { get; set; }
    public decimal? vlUltimaReavaliacao { get; set; }
    public decimal? vlLiquidoContabil { get; set; }
    public int? anosVidaUtil { get; set; }
    public decimal? taxaDepreciacaoAnual { get; set; }
    public string dtInicioDepreciacao { get; set; }
    public string dtUltimaReavaliacao { get; set; }
}

public class ContentBensGET
{
    public int? id { get; set; }
    public TipoBemBensGET tipoBem { get; set; }
    public GrupoBemBensGET grupoBem { get; set; }
    public EspecieBemBensGET especieBem { get; set; }
    public TipoUtilizacaoBensGET tipoUtilizacaoBem { get; set; }
    public TipoAquisicaoBensGET tipoAquisicao { get; set; }
    public FornecedorBensGET fornecedor { get; set; }
    public ResponsavelBensGET responsavel { get; set; }
    public EstadoConservacaoBensGET estadoConservacao { get; set; }
    public TipoComprovanteBensGET tipoComprovante { get; set; }
    public OrganogramaBensGET organograma { get; set; }
    public LocalizacaoFisicaBensGET localizacaoFisica { get; set; }
    public int? numeroRegistro { get; set; }
    public string numeroComprovante { get; set; }
    public string descricao { get; set; }
    public string dataInclusao { get; set; }
    public string dataAquisicao { get; set; }
    public bool? consomeCombustivel { get; set; }
    public int? qtdDiasGarantia { get; set; }
    public string dataInicioGarantia { get; set; }
    public string dataFinalGarantia { get; set; }
    public string dataProximaManutencao { get; set; }
    public string dtUltimaDepreciacao { get; set; }
    public SituacaoBemBensGET situacaoBem { get; set; }
    public string numeroPlaca { get; set; }
    public string observacao { get; set; }
    public NumeroAnoProcessoBensGET numeroAnoProcesso { get; set; }
    public NumeroAnoSolicitacaoBensGET numeroAnoSolicitacao { get; set; }
    public List<NumeroAnoEmpenhoBensGET> numeroAnoEmpenho { get; set; }
    public object bemImovel { get; set; }
    public BemValorBensGET bemValor { get; set; }
    public string numeroConvenio { get; set; }
}

public class ResponsavelBensGET
{
    public int id { get; set; }
}

public class EspecieBemBensGET
{
    public int id { get; set; }
}

public class EstadoConservacaoBensGET
{
    public int id { get; set; }
}

public class FornecedorBensGET
{
    public int id { get; set; }
}

public class GrupoBemBensGET
{
    public int id { get; set; }
}

public class LocalizacaoFisicaBensGET
{
    public int id { get; set; }
}

public class MetodoDepreciacaoBensGET
{
    public int id { get; set; }
}

public class MoedaBensGET
{
    public int id { get; set; }
    public string nome { get; set; }
    public string sigla { get; set; }
    public string dtCotacao { get; set; }
    public double fatorConversao { get; set; }
    public string formaCalculo { get; set; }
}

public class NumeroAnoEmpenhoBensGET
{
    public int id { get; set; }
    public string descricao { get; set; }
}

public class NumeroAnoProcessoBensGET
{
    public string descricao { get; set; }
}

public class NumeroAnoSolicitacaoBensGET
{
    public string descricao { get; set; }
}

public class OrganogramaBensGET
{
    public int id { get; set; }
}

public class SituacaoBemBensGET
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class TipoAquisicaoBensGET
{
    public int id { get; set; }
}

public class TipoUtilizacaoBensGET
{
    public int id { get; set; }
}

public class TipoBemBensGET
{
    public int id { get; set; }
}

public class TipoComprovanteBensGET
{
    public int id { get; set; }
}