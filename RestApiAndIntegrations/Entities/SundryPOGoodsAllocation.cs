using System;

namespace AD.CAAPS.Entities
{
    public class SundryPOGoodsAllocation
    {
        public int ID { get; set; }
        public int RecordID { get; set; }
        public int PoLineNumber { get; set; }
        public string PoLineTaxCode { get; set; }
        public string PoLineUOM { get; set; }
        public decimal? GrReceiptedValue { get; set; }
        public decimal? GrReceiptedQty { get; set; } 
        public string GrNumber { get; set; }
        public DateTime? GrReceivedDate { get; set; }
    }
}