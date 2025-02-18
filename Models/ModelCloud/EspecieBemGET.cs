﻿using System.Collections.Generic;

namespace MigraPatrim.Models.ModelCloud;

public class EspecieBemGET
{
    public int offset { get; set; }
    public int limit { get; set; }
    public bool hasNext { get; set; }
    public List<ContentEspecieBemGET> content { get; set; }
    public int total { get; set; }
    public object valor { get; set; }
    public object soma { get; set; }
    public object dados { get; set; }
}

public class ContentEspecieBemGET
{
    public int id { get; set; }
    public List<LinkEspecieBemGET> links { get; set; }
    public GrupoBemEspecieBemGET grupoBem { get; set; }
    public string descricao { get; set; }
}

public class GrupoBemEspecieBemGET
{
    public int id { get; set; }
}

public class LinkEspecieBemGET
{
    public string rel { get; set; }
    public string href { get; set; }
}