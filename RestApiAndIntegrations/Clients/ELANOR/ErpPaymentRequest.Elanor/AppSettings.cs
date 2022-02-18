using System.Collections.Generic;
using AD.CAAPS.Services;

namespace AD.CAAPS.ErpPaymentRequest.Elanor
{
    public class AppSettings
    {
        public string DateFormat { get; set; }
        public PaymentRequestServicesOptions PaymentRequestOptions { get; set; }
        // public bool LoadGLCodedLines { get; set; }
        // public bool LoadPOMatchedLines { get; set; }
        public bool UploadToFTP { get; set; }
        public Dictionary<string, string> FTPSettings { get; } = new Dictionary<string, string>();
        public bool SaveToFolder { get; set; }
        public string OutputFileLocation { get; set; }
        public Dictionary<string, string> FileSettings { get; } = new Dictionary<string, string>();
        public string DocumentStatusAfterExtract { get; set; }
        public string ExportFileDateFieldsFormat { get; set; }
        public int MaxRecords { get; set; } = 100;
        public bool DryRun { get; set; } = false;
        public AppSettings()
        {
            FTPSettings = new Dictionary<string, string>();
            FileSettings = new Dictionary<string, string>();
        }

    }
}