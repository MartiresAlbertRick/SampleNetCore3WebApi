namespace AD.CAAPS.ErpPaymentRequest.Porirua
{
    public class ExportData
    {
        public string CreditorNumber { get; set; }
        public string PoNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceDueDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceTotalAmount { get; set; }
        public decimal? GstAmount { get; set; }
        public decimal? WithholdingTaxAmount { get; set; }
        public string CaapsID { get; set; }
    }
}