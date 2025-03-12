namespace migracao-patrim.Models.ModelCloud;

public class TransferenciaPOST
{
    public TipoTransferenciaTransferenciaPOST tipoTransferencia { get; set; }
    public GrupoBemTransferenciaPOST grupoBem { get; set; }
    public EspecieBemTransferenciaPOST especieBem { get; set; }
    public ResponsavelTransferenciaPOST responsavel { get; set; }
    public OrganogramaTransferenciaPOST organograma { get; set; }
    public LocalizacaoFisicaTransferenciaPOST localizacaoFisica { get; set; }
    public TipoBemTransferenciaPOST tipoBem { get; set; }
    public int nroTransferencia { get; set; }
    public string dhTransferencia { get; set; }
    public string observacao { get; set; }
    public SituacaoTransferenciaPOST situacao { get; set; }
}

public class EspecieBemTransferenciaPOST
{
    public int id { get; set; }
}

public class GrupoBemTransferenciaPOST
{
    public int id { get; set; }
}

public class LocalizacaoFisicaTransferenciaPOST
{
    public int id { get; set; }
}

public class OrganogramaTransferenciaPOST
{
    public int id { get; set; }
}

public class ResponsavelTransferenciaPOST
{
    public int id { get; set; }
}

public class SituacaoTransferenciaPOST
{
    public string valor { get; set; }
    public string descricao { get; set; }
}

public class TipoBemTransferenciaPOST
{
    public int id { get; set; }
}

public class TipoTransferenciaTransferenciaPOST
{
    public int id { get; set; }
}