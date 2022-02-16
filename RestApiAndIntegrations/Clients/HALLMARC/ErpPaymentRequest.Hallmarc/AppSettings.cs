using AD.CAAPS.Services;
using System.Collections.Generic;

namespace AD.CAAPS.ErpPaymentRequest.Hallmarc
{
    public class AppSettings
    {
        public AppSettings()
        {
            FTPSettings = new Dictionary<string, string>();
            FileSettings = new Dictionary<string, string>();
            EntityGroups = new List<Dictionary<string, string>>();
        }

        public string DateFormat { get; set; }
        public PaymentRequestServicesOptions PaymentRequestOptions { get; set; }
        public bool UploadToFTP { get; set; }
        public Dictionary<string, string> FTPSettings { get; }
        public bool SaveToFolder { get; set; }
        public string OutputFileLocation { get; set; }
        public Dictionary<string, string> FileSettings { get; }
        public List<Dictionary<string, string>> EntityGroups { get; }
        public string DocumentStatusAfterExtract { get; set; }
        public string AccountsPayableAccountFormat { get; set; }
        public string ExportFileDateFieldsFormat { get; set; }
    }
}