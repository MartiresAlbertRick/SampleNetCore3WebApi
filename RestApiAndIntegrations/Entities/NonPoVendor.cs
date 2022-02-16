using System;

namespace AD.CAAPS.Entities
{
    public partial class NonPoVendor
    {
        public int ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string VendorBusinessNumber { get; set; }
        public string VendorName { get; set; }
        public string FirstRecipientType { get; set; }
        public string FirstRecipient { get; set; }
        public string FirstRecipientFullName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitName { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string DefaultGLCode { get; set; }
        public string AccountNumber { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string VendorBusinessNumberNormalized { get; set; }
        public string AccountNumberNormalized { get; set; }
        public string UpdatedByUser { get; set; }
        public string RoutingCode { get; set; }
        public string PoNumber { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}