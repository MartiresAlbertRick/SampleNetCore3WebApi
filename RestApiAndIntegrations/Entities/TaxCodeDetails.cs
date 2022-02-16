using System;

namespace AD.CAAPS.Entities
{
    public class TaxCodeDetails
    {
        public int ID { get; set; }
        public string TaxCode { get; set; }
        public decimal? TaxRate { get; set; }
        public string TaxDescription { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string UpdatedByUser { get; set; }
        public string ProcessingCurrency { get; set; }
        public bool? DefaultTaxCodeFlag { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}