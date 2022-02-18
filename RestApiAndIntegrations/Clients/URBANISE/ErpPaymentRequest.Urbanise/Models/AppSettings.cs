using AD.CAAPS.Services;

namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    public class AppSettings
    {
        public string EntityFrameworkDateFormat { get; set; }
        public PaymentRequestServicesOptions PaymentRequestOptions { get; set; } = new PaymentRequestServicesOptions();
        // public bool LoadGLCodedLines { get; set; }
        // public bool LoadPOMatchedLines { get; set; }
        public string PDFUploadEndpoint { get; set; }
        public string PDFUploadEndpointPropertyIdRequired { get; set; }
        public string PaymentRequestExportEndpoint { get; set; }
        public string ClientBearerToken { get; set; }
        public string CaapsApiDownloadInvoiceDocumentEndpoint { get; set; }
        public string XClientId { get; set; }
        public string OcpApimTrace { get; set; }
        public string OcpApimSubscriptionKey { get; set; }
        public string DocumentStatusAfterExtract { get; set; }
        public bool IsImportConfirmationExpected { get; set; }
        public string ExportDateFormat { get; set; }
        public int MaxRecords { get; set; } = 100;
        public bool DryRun { get; set; } = false;
        public string TempDownloadPath { get; set; }
    }
}