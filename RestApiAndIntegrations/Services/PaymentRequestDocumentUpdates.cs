using System;

namespace AD.CAAPS.Services
{
    public class PaymentRequestDocumentUpdates
    {
        public bool Success { get; set; } = false;
        public int ID { get; set; }
        public string Status { get; set; }
        public DateTime? ExportDate { get; set; }
        public bool IsImportConfirmationExpected { get; set; }
        public DateTime? ErpImportDate { get; set; }
        public string UserComments { get; set; }
        public string FileName { get; set; }
        public string ExportPayload { get; set; }
        public string ExportResponse { get; set; }
    }
}
