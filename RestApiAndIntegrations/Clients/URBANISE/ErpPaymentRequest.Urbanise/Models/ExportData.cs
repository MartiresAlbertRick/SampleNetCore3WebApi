namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    public class ExportData
    {
        public int Id { get; set; }
        public int? PropertyId { get; set; }
        public int? Status { get; set; }
        public decimal? Amount { get; set; }
        public string BpayReference { get; set; }
        public string DueDate { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string Note { get; set; }
        public int? PayType { get; set; }
        public string Reference { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
    }
}