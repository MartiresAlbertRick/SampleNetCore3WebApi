using System;

namespace AD.CAAPS.Entities
{
    public class InvoiceLifeCycleEvent
    {
        public int ID { get; set; }
        public int? RecordID { get; set; }
        public DateTime? ClientApprovalDate { get; set; }
        public DateTime? ExportDate { get; set; }
        public DateTime? PaymentNotificationDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? OwnershipTakenDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? ErpImportDate { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        public DateTime? ImportedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}