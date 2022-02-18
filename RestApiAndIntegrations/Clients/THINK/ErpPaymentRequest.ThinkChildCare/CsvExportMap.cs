using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration;
using AD.CAAPS.ErpPaymentRequest.Common;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    class CsvExportMap: ClassMap<ExportRow>
    {
        public CsvExportMap(string dateFormat) {
            var csvDateConverter = new CsvDateConverter(String.IsNullOrWhiteSpace(dateFormat) ? "dd/MM/yyyy" : dateFormat);
            Map(m => m.CaapsId).Name("CAAPS_ID");
            Map(m => m.InvoiceNumber).Name("Document number/Invoice number");
            Map(m => m.DocumentType).Name("Type");
            Map(m => m.VendorCode).Name("VENDOR_CODE");
            Map(m => m.EntityCode).Name("ENTITY_CODE");
            Map(m => m.BranchCode).Name("Site");
            Map(m => m.AccountDate).Name("Account date").TypeConverter(csvDateConverter);
            Map(m => m.IssueDate).Name("Source date").TypeConverter(csvDateConverter);
            Map(m => m.LineNumber).Name("Line number");
            Map(m => m.GlCode).Name("GL");
            Map(m => m.Description).Name("Des");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.TaxCode).Name("Line Tax Code");
            Map(m => m.DimensionAreaManager).Name("Dimension _Area manager");
            Map(m => m.DimensionPlaces).Name("Dimension _Places");
            Map(m => m.DimensionRegion).Name("Dimension _Region");
            Map(m => m.DimensionProjects).Name("Dimension _Projects");
            Map(m => m.DimensionRoom).Name("Dimension _Room");
            Map(m => m.CaapsUrl).Name("CAAPS_URL");
        }
    }
}
