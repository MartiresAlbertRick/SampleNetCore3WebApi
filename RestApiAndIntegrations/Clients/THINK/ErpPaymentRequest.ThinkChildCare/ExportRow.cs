using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    class ExportRow
    {
        public string CaapsId { get; set; }
        public string InvoiceNumber { get; set; }
        public string DocumentType { get; set; }
        public string VendorCode { get; set; }
        public string EntityCode { get; set; }
        public string BranchCode { get; set; }
        public DateTime? AccountDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public int? LineNumber { get; set; }
        public string GlCode { get; set; }
        public string Description { get; set; }
        public Decimal? Amount { get; set; }
        public string TaxCode { get; set; }
        public string DimensionAreaManager { get; set; }
        public string DimensionPlaces { get; set; }
        public string DimensionRegion { get; set; }
        public string DimensionProjects { get; set; }
        public string DimensionRoom { get; set; }
        public string CaapsUrl { get; set; }
    }
}
