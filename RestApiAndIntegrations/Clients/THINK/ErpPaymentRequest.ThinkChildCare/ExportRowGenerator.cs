using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using NLog;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    class ExportRowGenerator
    {
        private readonly PaymentRequestServices paymentRequestServices;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ExportRowGenerator(PaymentRequestServices paymentRequestServices)
        {
            this.paymentRequestServices = paymentRequestServices ?? throw new ArgumentNullException(nameof(paymentRequestServices), $"The {GetType().Name} class requires an instance of {nameof(PaymentRequestServices)}.");
        }
        private async Task AppendExportRows(PaymentRequestHeader header, IList<ExportRow> exportRows)
        {
            logger.Debug(() => $"Creating export rows for payment request {header.CaapsRecordId}.");
            int createdRowCount = 0;
            string caapsUrl = await paymentRequestServices.GetFileURL(header.ID).ConfigureAwait(false);
            int lineIndex = 1;
            foreach (GLCodeLine line in header.PaymentRequestGLCodedLines)
            {
                var exportRow = new ExportRow()
                {
                    CaapsId = header.CaapsRecordId,
                    InvoiceNumber = header.DocRefNumberA,
                    Amount = line.LineAmountTotal,
                    Description = line.LineDescription,
                    DocumentType = header.DocType,
                    EntityCode = header.EntityCode,
                    BranchCode = header.BranchCode,
                    GlCode = line.GLCode,
                    AccountDate = header.DocDateIssued, // Yes, we are deploying the same data in two different columns
                    IssueDate = header.DocDateIssued,
                    LineNumber = lineIndex++,
                    TaxCode = line.LineTaxCode,
                    VendorCode = header.VendorCode,
                    CaapsUrl = caapsUrl
                };
                logger.Trace(() => $"Export row: CAAPS ID=\"{exportRow.CaapsId}\". InvoiceNumber=\"{exportRow.InvoiceNumber}\". Amount={exportRow.Amount}. Description=\"{exportRow.Description}\". DocumentType=\"{exportRow.DocumentType}\". BranchCode=\"{exportRow.BranchCode}\". GlCode=\"{exportRow.GlCode}\". AccountDate = {exportRow.AccountDate}. IssueDate={exportRow.IssueDate}. LineNumber={exportRow.LineNumber}. TaxCode=\"{exportRow.TaxCode}\". VendorCode=\"{exportRow.VendorCode}\".");
                exportRows.Add(exportRow);
                createdRowCount++;
            }
            logger.Debug(() => $"Created {createdRowCount} for payment request {header.CaapsRecordId}.");
        }

        public async Task<IList<ExportRow>> CreateExportRows(IList<PaymentRequestHeader> paymentRequestHeaders)
        {
            logger.Debug($"Creating export rows for payment request headers. Count: {paymentRequestHeaders.Count}.");
            IList<ExportRow> result = new List<ExportRow>();
            foreach (PaymentRequestHeader header in paymentRequestHeaders)
            {
                await AppendExportRows(header, result);
            }
            logger.Debug($"Export rows creation finished. Count of export rows created: {result.Count}.");
            return result;
        }
    }
}
