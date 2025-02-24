namespace MigraPatrim.Models.ModelPostgres;

public class Fornecedor
{
    public int id { get; set; }
    public string id_cloud { get; set; }
    public int i_fornec { get; set; }
    public string nome { get; set; }
    public string cpf_cnpj { get; set; }
    public string tipo { get; set; }
    public string end_logradouro { get; set; }
    public string end_numero { get; set; }
    public string end_bairro { get; set; }
    public string end_cep { get; set; }
    public string end_cidade { get; set; }
    public string end_uf { get; set; }
    public string telefone { get; set; }
    public string email { get; set; }
}
