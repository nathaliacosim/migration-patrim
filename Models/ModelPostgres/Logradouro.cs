namespace migracao-patrim.Models.ModelPostgres;

public class Logradouro
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int i_fornec { get; set; }
    public string logradouro { get; set; }
    public string logradouro_atual { get; set; }
    public string numero { get; set; }
    public string bairro { get; set; }
    public string cidade { get; set; }
    public string estado { get; set; }
    public string cep { get; set; }
    public int id_tipo_logradouro { get; set; }
    public string tipo_logradouro { get; set; }
    public int id_cidade { get; set; }
    public int id_estado { get; set; }
    public int id_bairro { get; set; }
}
