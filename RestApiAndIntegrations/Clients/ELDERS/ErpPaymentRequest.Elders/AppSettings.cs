using AD.CAAPS.Services;
using System.Collections.Generic;

namespace AD.CAAPS.ErpPaymentRequest.Elders
{
    public class AppSettings
    {
        public string EFConfigurationDateFormat { get; set; }
        public string ExportDateFormat { get; set; }
        public PaymentRequestServicesOptions PaymentRequestOptions { get; set; }
        public string DataUploadEndpoint { get; set; }
        public string XSasToken { get; set; }
        public string ExportFileDateFieldsFormat { get; set; }
        public string DocumentStatusAfterExtract { get; set; }
        public Dictionary<string, string> IDocHeaders { get; } = new Dictionary<string, string>();
    }
}