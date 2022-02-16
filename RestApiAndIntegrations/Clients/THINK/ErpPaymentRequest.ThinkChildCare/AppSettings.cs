using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AD.CAAPS.ErpPaymentRequest.Common;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
#pragma warning disable CA1812 
    // Instantiated implicitly in AD.CAAPS.ErpPaymentRequest.ThinkChildCare.Program.CreateAppSettings
    public class AppSettings : BaseAppSettings
    {
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string ConnectionStringName { get; set; } = "Default";
        public bool IsImportConfirmationExpected { get; set; } = false;
        public string DocumentStatusAfterExtract { get; set; } = "EXPORTED";
        public string LocalTargetFolder { get; set; } = Path.GetTempPath();
        public bool UploadToFTP { get; set; } = false;
        public FTPSettings FtpSettings { get; } = new FTPSettings();
    }
#pragma warning restore CA1812
}
