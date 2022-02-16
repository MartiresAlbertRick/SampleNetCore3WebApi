using Newtonsoft.Json;
using System.Collections.Generic;

namespace AD.CAAPS.ErpVendorPortalUpload.JLLGST
{
    public class ExportList
    {
        [JsonProperty(PropertyName = "data")]
        public List<ExportData> Data { get; } = new List<ExportData>();

        public void SetData(List<ExportData> data)
        {
            Data.Clear();
            Data.AddRange(data);
        }
    }

    public class ExportData
    {
        [JsonIgnore]
        public int? ID { get; set; }
        [JsonProperty(PropertyName = "vendor_code")]
        public string VendorCode { get; set; }
        [JsonProperty(PropertyName = "vendor_name")]
        public string VendorName { get; set; }
        [JsonProperty(PropertyName = "vendor_email_id")]
        public string VendorEmailID { get; set; }
        [JsonProperty(PropertyName = "client_name")]
        public string ClientName { get; set; }
        [JsonProperty(PropertyName = "caaps_id")]
        public string CaapsID { get; set; }
        [JsonProperty(PropertyName = "doc_ref_number")]
        public string DocRefNumber { get; set; }
        [JsonProperty(PropertyName = "po_number")]
        public string PoNumber { get; set; }
        [JsonProperty(PropertyName = "po_processing_status")]
        public string PoProcessingStatus { get; set; }
        [JsonProperty(PropertyName = "processing_status_date")]
        public string ProcessingStatusDate { get; set; }
        [JsonProperty(PropertyName = "processing_remarks")]
        public string ProcessingRemarks { get; set; }
        [JsonProperty(PropertyName = "business_unit")]
        public string BusinessUnit { get; set; }
        [JsonProperty(PropertyName = "date_issued")]
        public string DateIssued { get; set; }
        [JsonProperty(PropertyName = "received_date")]
        public string ReceivedDate { get; set; }
        [JsonProperty(PropertyName = "sgst_utgst_amount")]
        public double? SgstUtgstAmount { get; set; }
        [JsonProperty(PropertyName = "cgst_amount")]
        public double? CgstAmount { get; set; }
        [JsonProperty(PropertyName = "igst_amount")]
        public double? IgstAmount { get; set; }
        [JsonProperty(PropertyName = "amount_excluding_tax")]
        public double? AmountExcludingTax { get; set; }
        [JsonProperty(PropertyName = "tax_amount")]
        public double? TaxAmount { get; set; }
        [JsonProperty(PropertyName = "total_amount")]
        public double? TotalAmount { get; set; }
        [JsonProperty(PropertyName = "vendor_state")]
        public string VendorState { get; set; }
        [JsonProperty(PropertyName = "business_unit_state")]
        public string BusinessUnitState { get; set; }
        [JsonProperty(PropertyName = "document_type")]
        public string DocumentType { get; set; }
        [JsonProperty(PropertyName = "record_owner")]
        public string RecordOwner { get; set; }
        [JsonProperty(PropertyName = "Cess")]
        public double? Cess { get; set; }
        [JsonProperty(PropertyName = "tax_code")]
        public string TaxCode { get; set; }
        [JsonProperty(PropertyName = "due_date")]
        public string DueDate { get; set; }
        [JsonProperty(PropertyName = "paid_date")]
        public string PaidDate { get; set; }
        [JsonProperty(PropertyName = "fm_approval_date")]
        public string FmApprovalDate { get; set; }
        [JsonProperty(PropertyName = "finance_manager_approval_date")]
        public string FinanceManagerApprovalDate { get; set; }
        [JsonProperty(PropertyName = "fsc_approval_date")]
        public string FscApprovalDate { get; set; }
        [JsonProperty(PropertyName = "gr_approval_date")]
        public string GrApprovalDate { get; set; }
        [JsonProperty(PropertyName = "client_approval_date")]
        public string ClientApprovalDate { get; set; }
        [JsonProperty(PropertyName = "caaps_export_to_jde_date")]
        public string CaapsExportToJdeDate { get; set; }
        [JsonProperty(PropertyName = "jde_payment_notification_date")]
        public string JdePaymentNotificationDate { get; set; }
        [JsonProperty(PropertyName = "last_action_date")]
        public string LastActionDate { get; set; }
        [JsonProperty(PropertyName = "client_payment_due_date")]
        public string ClientPaymentDueDate { get; set; }
        [JsonProperty(PropertyName = "expected_vendor_payment_date")]
        public string ExpectedVendorPaymentDate { get; set; }
        [JsonProperty(PropertyName = "payment_number")]
        public string PaymentNumber { get; set; }
        [JsonProperty(PropertyName = "payment_amount")]
        public double? PaymentAmount { get; set; }
        [JsonProperty(PropertyName = "witholding_taxes")]
        public string WithholdingTaxes { get; set; }
        [JsonProperty(PropertyName = "email_circulated_on_date_time_in_ist")]
        public string EmailCirculatedOnDateTimeInIst { get; set; }
        [JsonProperty(PropertyName = "client_invoice_number")]
        public string ClientInvoiceNumber { get; set; }
        [JsonProperty(PropertyName = "files")]
        public List<File> Files { get; } = new List<File>();

        public void SetFiles(List<File> files)
        {
            Files.Clear();
            Files.AddRange(files);
        }
    }

    public class File
    {
        [JsonProperty(PropertyName = "file_path")]
        public string DestinationFilePath { get; set; }
        [JsonIgnore]
        public string SourceFilePath { get; set; }
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }
    }
}