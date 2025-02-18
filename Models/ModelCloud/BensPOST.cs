namespace MigraPatrim.Models.ModelCloud;

public class BensPOST
{
    public int id { get; set; }
    public TipoBemBensPOST tipoBem { get; set; }
    public GrupoBemBensPOST grupoBem { get; set; }
    public EspecieBemBensPOST especieBem { get; set; }
    public OrganogramaBensPOST organograma { get; set; }
    public FornecedorBensPOST fornecedor { get; set; }
    public ResponsavelBensPOST responsavel { get; set; }
    public TipoAquisicaoBensPOST tipoAquisicao { get; set; }
    public EstadoConservacaoBensPOST estadoConservacao { get; set; }
    public string descricao { get; set; }
    public string numeroPlaca { get; set; }
    public int numeroRegistro { get; set; }
    public string dataInclusao { get; set; }
    public string dataAquisicao { get; set; }
    public bool consomeCombustivel { get; set; }
    public SituacaoBemBensPOST situacaoBem { get; set; }
    public BemValorBensPOST bemValor { get; set; }
}

public class OrganogramaBensPOST
{
    public int id { get; set; }
}

public class ResponsavelBensPOST
{
    public int id { get; set; }
}

public class FornecedorBensPOST
{
    public int id { get; set; }
}

public class BemValorBensPOST
{
    public int? id { get; set; } = null;
    public MetodoDepreciacaoBensPOST metodoDepreciacao { get; set; } = new MetodoDepreciacaoBensPOST();
    public MoedaBensPOST moeda { get; set; } = new MoedaBensPOST();
    public decimal? vlAquisicao { get; set; } = null;
    public decimal? vlAquisicaoConvertido { get; set; } = null;
    public decimal? vlResidual { get; set; } = null;
    public decimal? vlDepreciavel { get; set; } = null;
    public decimal? vlDepreciado { get; set; } = null;
    public decimal? saldoDepreciar { get; set; } = null;
    public decimal? capacidadeProdutiva { get; set; } = null;
    public decimal? vlUltimaReavaliacao { get; set; } = null;
    public decimal? vlLiquidoContabil { get; set; } = null;
    public int? anosVidaUtil { get; set; } = null;
    public decimal? taxaDepreciacaoAnual { get; set; } = null;
    public string? dtInicioDepreciacao { get; set; } = null;
    public string? dtUltimaReavaliacao { get; set; } = null;
}

public class MoedaBensPOST
{
    public int id { get; set; }
    public string nome { get; set; } = string.Empty;
    public string sigla { get; set; } = string.Empty;
    public string dtCotacao { get; set; }
    public decimal fatorConversao { get; set; }
    public string formaCalculo { get; set; } = string.Empty;
}

public class MetodoDepreciacaoBensPOST
{
    public int id { get; set; }
}

public class SituacaoBemBensPOST
{
    public string valor { get; set; } = string.Empty;
    public string descricao { get; set; } = string.Empty;
}

public class EstadoConservacaoBensPOST
{
    public int id { get; set; }
}

public class TipoAquisicaoBensPOST
{
    public int id { get; set; }
}

public class TipoUtilizacaoBemBensPOST
{
    public int? id { get; set; }
}

public class TipoBemBensPOST
{
    public int id { get; set; }
}

public class GrupoBemBensPOST
{
    public int id { get; set; }
}

public class EspecieBemBensPOST
{
    public int id { get; set; }
}