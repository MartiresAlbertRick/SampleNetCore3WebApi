using System;

namespace AD.CAAPS.Entities
{
    public partial class ClosedPurchaseOrder
    {
        public int ID { get; set; }
        public string PoNumber { get; set; }
        public decimal? PoAmountTotal { get; set; }
        public string VendorCode { get; set; }
        public string PoRaisedDate { get; set; }
        public string EntityCode { get; set; }
        public string PoType { get; set; }
        public string PoDate { get; set; }
        public string PoRaisedByUsername { get; set; }
        public string JobNumber { get; set; }
        public string CurrencyCode { get; set; }
        public string PoStatusCode { get; set; }
        public decimal? PoOpenAmount { get; set; }
        public string PayToVendorCode { get; set; }
        public string SiteCode { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}