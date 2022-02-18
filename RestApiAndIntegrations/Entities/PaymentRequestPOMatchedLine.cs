using System;

namespace AD.CAAPS.Entities
{
    public partial class PaymentRequestPOMatchedLine : LineItem
    {
        public string CaapsRecordId { get; set; }
        public string PoNumber { get; set; }
        public string DocRefNumberA { get; set; }
        public DateTime? DocDateIssued { get; set; }
        public string VendorCode { get; set; }
        public decimal? DocAmountTotal { get; set; }
        public PurchaseOrder AllocatedPurchaseOrder { get; set; } = new PurchaseOrder();
    }
}