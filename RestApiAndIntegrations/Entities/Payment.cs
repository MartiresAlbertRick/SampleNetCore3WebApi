using System;

namespace AD.CAAPS.Entities
{
    public partial class Payment
    {
        public int ID { get; set; }
        public string CaapsUniqueId { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ClientTransactionId { get; set; }
        public string PaymentVoucherNumber { get; set; }
        public string PaymentBatchNumber { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}