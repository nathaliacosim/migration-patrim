namespace migracao-patrim.Models.ModelCloud
{
    public class BemTransferenciaBemPOST
    {
        public int id { get; set; }
    }

    public class TransferenciaBemPOST
    {
        public BemTransferenciaBemPOST bem { get; set; }
        public TransferenciaTransferenciaBemPOST transferencia { get; set; }
    }

    public class TransferenciaTransferenciaBemPOST
    {
        public int id { get; set; }
    }
}
