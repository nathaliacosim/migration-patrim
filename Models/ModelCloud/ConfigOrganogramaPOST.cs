using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class ConfigOrganogramaPOST
{
    public string descricao { get; set; }
    public string mascara { get; set; }
    public List<NiveisConfigOrganogramaPOST> niveis { get; set; }
}

public class NiveisConfigOrganogramaPOST
{
    public int nivel { get; set; }
    public string descricao { get; set; }
    public int digitos { get; set; }
    public string? separador { get; set; }
}

