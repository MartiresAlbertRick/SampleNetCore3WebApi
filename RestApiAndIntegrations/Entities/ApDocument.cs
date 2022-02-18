using System;
using System.Collections.Generic;

namespace AD.CAAPS.Entities
{
    public partial class ApDocument
    {
        public ApDocument()
        {
            LineItems = new List<LineItem>();
            AccountCodingLines = new List<GLCodeLine>();
        }

        public int ID { get; set; }
        public string CaapsRecordId { get; set; }
        public DateTime? RecordCreatedDate { get; set; }
        public string DocReceivedSource { get; set; }
        public string DocReceivedType { get; set; }
        public DateTime? DocDateIssued { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string DocRefNumberA { get; set; }
        public string DocRefNumberB { get; set; }
        public int? ImportBatchID { get; set; }
        public string RoleIDs { get; set; }
        public string UniqueIdentifier { get; set; }
        public string DocHeaderUID { get; set; }
        public string DocType { get; set; }
        public string VendorBusinessNumber { get; set; }
        public string ProcessType { get; set; }
        public decimal? DocAmountTaxEx { get; set; }
        public decimal? DocAmountTax { get; set; }
        public decimal? DocAmountTotal { get; set; }
        public string DocAmountCurrency { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string PoNumber { get; set; }
        public string ProcessStatusCurrent { get; set; }
        public string DocDescription { get; set; }
        public DateTime? DocDateReceived { get; set; }
        public DateTime? ExportDate { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }

        public string FileName { get; set; }
#pragma warning disable CA1056 // Uri properties should not be strings - This maps to the DRAWINGS table field name and is a string
        public string FileURL { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
        public string FromEmailAddress { get; set; }
        public string AccountNumber { get; set; }
        public string StatementInvoiceNumber { get; set; }
        public string VendorBankBsb { get; set; }
        public string VendorBankAccountNumber { get; set; }
        public string BPAYBillerCode { get; set; }
        public string BPAYReferenceNumber { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTermsCode { get; set; }
        public string RoutingCode { get; set; }
        public int? DocLineItemCount { get; set; }
        public bool? DocMultiplePoInHeaderYN { get; set; }
        public DateTime? DocDateDue { get; set; }
        public string DocPaymentType { get; set; }

        public IList<LineItem> LineItems { get; }
        public IList<GLCodeLine> AccountCodingLines { get; } 
    }
}