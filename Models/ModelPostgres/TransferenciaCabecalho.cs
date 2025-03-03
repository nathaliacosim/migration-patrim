namespace MigraPatrim.Models.ModelPostgres;

public class TransferenciaCabecalho
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int id_cloud_registro { get; set; }
    public int id_tipo_transferencia { get; set; }
    public string mes { get; set; }
    public string ano { get; set; }
    public string mes_ano { get; set; }
    public string data_transferencia { get; set; }
    public string tipo_transferencia { get; set; }
    public string novo1 { get; set; }
    public string novo2 { get; set; }
    public string observacao { get; set; }
}
