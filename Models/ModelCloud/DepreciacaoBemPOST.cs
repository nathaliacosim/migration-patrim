namespace MigraPatrim.Models.ModelCloud
{
    public class BemDepreciacaoBemPOST
    {
        public int id { get; set; }
    }

    public class DepreciacaoDepreciacaoBemPOST
    {
        public int id { get; set; }
    }

    public class DepreciacaoBemPOST
    {
        public DepreciacaoDepreciacaoBemPOST depreciacao { get; set; }
        public BemDepreciacaoBemPOST bem { get; set; }
        public decimal? vlDepreciado { get; set; }
        public string notaExplicativa { get; set; }
    }
}
