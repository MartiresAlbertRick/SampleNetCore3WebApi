using System;

namespace AD.CAAPS.Entities
{
    public partial class GoodsReceipt
    {
        public int ID { get; set; }
        public string GoodsReceivedNumber { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string PoNumber { get; set; }
        public string PoLineNumber { get; set; }
        public decimal? ReceiptedQty { get; set; }
        public decimal? ReceiptedValueTaxEx { get; set; }
        public string EntityCode { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}