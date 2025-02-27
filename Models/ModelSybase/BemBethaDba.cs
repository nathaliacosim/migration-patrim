namespace MigraPatrim.Models.ModelSybase;

public class BemBethaDba
{
    public int? i_bem { get; set; }
    public int? i_conta { get; set; }
    public int? i_custo { get; set; }
    public int? i_unidade { get; set; }
    public int? i_respons { get; set; }
    public int? i_fornec { get; set; }
    public int? i_tipo_natur { get; set; }
    public string descricao { get; set; }
    public string data_aquis { get; set; }
    public string documento { get; set; }
    public string fornecedor { get; set; }
    public decimal? valor_aquis { get; set; }
    public int? i_moeda { get; set; }
    public decimal? valor_original { get; set; }
    public decimal? valor_reav { get; set; }
    public decimal? valor_depr { get; set; }
    public string tipo_aquis { get; set; }
    public string data_garant { get; set; }
    public string data_proxrev { get; set; }
    public int? nr_empenho { get; set; }
    public string situacao { get; set; }
    public string historico { get; set; }
    public string uso { get; set; }
    public string est_cons { get; set; }
    public int? ano_empenho { get; set; }
    public int? i_ano_proc { get; set; }
    public int? i_processo { get; set; }
    public string data_processo { get; set; }
    public string liquid_compras { get; set; }
    public int? codigo_localizacao { get; set; }
    public int? codigo_intervencao { get; set; }
    public string numero_placa { get; set; }
    public string numero_termo { get; set; }
    public string data_termo { get; set; }
    public string data_prazo { get; set; }
    public decimal? valor_residual { get; set; }
    public decimal? valor_depreciavel { get; set; }
    public string dt_inicio_deprec { get; set; }
    public decimal perc_deprec_anual { get; set; }
    public string utiliza_exprecnatur { get; set; }
    public int? i_localiz_fis { get; set; }
    public int? i_entidades { get; set; }
    public int? i_detalhamentos_bens { get; set; }
    public int? utilizacao { get; set; }
    public int? origem { get; set; }
    public int? i_tipo_documento { get; set; }
    public int? vida_util { get; set; }
}