using System;

namespace AD.CAAPS.Entities
{
    public partial class PaymentRequestGLCodedLine : GLCodeLine
    {
        public string CaapsRecordId { get; set; }
        public string DocRefNumberA { get; set; }
        public DateTime? DocDateIssued { get; set; }
        public string VendorCode { get; set; }
        public decimal? DocAmountTotal { get; set; }
    }
}