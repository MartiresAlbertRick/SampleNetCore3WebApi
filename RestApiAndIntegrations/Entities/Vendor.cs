using System;

namespace AD.CAAPS.Entities
{
    public partial class Vendor
    {
        public int ID { get; set; }
        public string VendorUID { get; set; }
        public string EntityCode { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string VendorBusinessNumber { get; set; }
        public string VendorARContactName { get; set; }
        public string VendorARContactEmailAddress { get; set; }
        public string VendorAddressLine01 { get; set; }
        public string VendorAddressLine02 { get; set; }
        public string VendorAddressLine03 { get; set; }
        public string VendorAddressLine04 { get; set; }
        public string VendorSuburb { get; set; }
        public string VendorState { get; set; }
        public string VendorPostCode { get; set; }
        public string VendorCity { get; set; }
        public string VendorCountry { get; set; }
        public string BankBsbNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public string BPAYBillerCode { get; set; }
        public string BPAYReferenceNumber { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTermsCode { get; set; }
        public string VendorCurrencyCode { get; set; }
        public string VendorBankAccountList { get; set; }
        public bool? PoRequiredYN { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}