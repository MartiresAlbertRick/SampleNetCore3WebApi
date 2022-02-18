using System;

namespace AD.CAAPS.Entities
{
    public class PaymentTerms
    {
        public int ID { get; set; }
        public string PaymentTermsCode { get; set; }
        public string PaymentTermsDescription { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}