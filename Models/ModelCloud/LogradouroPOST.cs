namespace MigraPatrim.Models.ModelCloud;

public class LogradouroPOST
{
    public MunicipioLogradouroPOST municipio { get; set; }
    public TipoLogradouroLogradouroPOST tipoLogradouro { get; set; }
    public string nome { get; set; }
    public string cep { get; set; }
}

public class TipoLogradouroLogradouroPOST
{
    public int id { get; set; }
}

public class MunicipioLogradouroPOST
{
    public int id { get; set; }
}