using AD.CAAPS.Services;

namespace AD.CAAPS.ErpPaymentRequest.Porirua
{
    public class AppSettings
    {
        public string DateFormat { get; set; }
        public PaymentRequestServicesOptions PaymentRequestOptions { get; set; }
        public string PaymentRequestExportEndpoint { get; set; }
        public string PDFUploadEndpoint { get; set; }
        public string PaymentRequestCompletionEndpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CaapsApiDownloadInvoiceDocumentEndpoint { get; set; }
        public string XClientId { get; set; }
        public string OcpApimTrace { get; set; }
        public string OcpApimSubscriptionKey { get; set; }
        public string DocumentStatusAfterExtract { get; set; }
        public bool IsImportConfirmationExpected { get; set; }
        public string TempDownloadPath { get; set; }
    }
}