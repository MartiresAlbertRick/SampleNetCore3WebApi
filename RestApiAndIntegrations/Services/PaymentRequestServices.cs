using AD.CAAPS.Entities;
using AD.CAAPS.Repository;
using Microsoft.EntityFrameworkCore;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using AD.CAAPS.Common;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace AD.CAAPS.Services
{
    public class PaymentRequestServices : BaseServices
    {
        private readonly PaymentRequestServicesOptions options;
        const string PENDINGEXPORTSTATUS = "PENDING EXPORT", USERNAME = "SYSTEM", ACTIONNAME = "PAYMENT_REQUEST";
        const int MANAGEDTABLEID = 0;
        private readonly SystemOptionServices systemOptionServices;
        private bool? isSso = null;
        private string webServerUrl;

        public PaymentRequestServices(DBConfiguration configuration, PaymentRequestServicesOptions options) : base(configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            systemOptionServices = CreateSystemOptionServices();
        }

        public PaymentRequestServices(CAAPSDbContext Context, PaymentRequestServicesOptions options) : base(Context)
        {
            if (Context is null)
            {
                throw new ArgumentNullException(nameof(Context));
            }
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            systemOptionServices = CreateSystemOptionServices();
        }

        private SystemOptionServices CreateSystemOptionServices()
        {
            return new SystemOptionServices(Context);
        }

        private async Task<bool> IsSSO()
        {
            if (isSso == true)
            {
                return true;
            } else if (isSso == false)
            {
                return false;
            } else
            {
                const string optionName = "CAAPS Web Server Use SSO";
                logger.Debug($"Checking if the web server uses SSO. Getting system option {optionName}.");
                SystemOption systemOption = await systemOptionServices.GetSystemOptionByName(optionName);
                if (systemOption == null)
                {
                    logger.Debug($"Could not find option {optionName}. The system does not use SSO.");
                    isSso = false;
                    return false;
                }
                else
                {
                    string optionValue = systemOption.OptionValue;
                    bool result = String.IsNullOrWhiteSpace(optionValue) ? false : String.Compare("Y", optionValue.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
                    string useClause = result ? "uses" : "does not use";
                    logger.Debug($"System {useClause} SSO. {optionName} = \"{optionValue}\".");
                    isSso = result;
                    return result;
                }

            }
        }

        private async Task<string> GetWebServerUrl()
        {
            if (String.IsNullOrWhiteSpace(webServerUrl))
            {
                const string optionName = "CAAPS Web Server Route";
                logger.Debug($"Getting the web server route. Getting system option {optionName}.");
                SystemOption systemOption = await systemOptionServices.GetSystemOptionByName(optionName);
                if (systemOption == null)
                {
                    throw new SystemOptionMissingException("Unable to retrieve the web server route from the system options. System option \"{optionName}\" does not exist.");
                }
                else if (String.IsNullOrWhiteSpace(systemOption.OptionValue))
                {
                    throw new SystemOptionMissingException("Web server route is empty. System option \"{optionName}\" exists but does not have a value.");
                }
                else
                {
                    webServerUrl = systemOption.OptionValue;
                }
            }
            return webServerUrl;
        }

        public async Task<IList<PaymentRequestHeader>> GetPaymentRequests(int maxRecords = -1)
        {
            return await GetPaymentRequests(-1, maxRecords);
        }

        public async Task<IList<PaymentRequestHeader>> GetPaymentRequests(int recordID, int maxRecords = -1)
        {
            logger.Debug($"Attempting to retrieve records from CAAPSDbContext - ApDocuments DbSet. RecordID: {recordID}, MaxRecords limit: {maxRecords:N0}.");

            var query = (from t
                          in Context.ApDocuments
                                where t.ProcessStatusCurrent == PENDINGEXPORTSTATUS
                                orderby t.ID
                                select new PaymentRequestHeader
                                {
                                    ID = t.ID,
                                    CaapsRecordId = t.CaapsRecordId,
                                    DocDateIssued = t.DocDateIssued,
                                    DocRefNumberA = t.DocRefNumberA,
                                    ProcessType = t.ProcessType,
                                    DocAmountTotal = t.DocAmountTotal,
                                    EntityCode = t.EntityCode,
                                    BranchCode = t.BranchCode,
                                    BusinessUnitCode = t.BusinessUnitCode,
                                    SiteCode = t.SiteCode,
                                    DivisionCode = t.DivisionCode,
                                    PoNumber = t.PoNumber,
                                    DocAmountTax = t.DocAmountTax,
                                    DocDescription = t.DocDescription,
                                    DocAmountCurrency = t.DocAmountCurrency,
                                    DocAmountTaxEx = t.DocAmountTaxEx,
                                    VendorCode = t.VendorCode,
                                    VendorName = t.VendorName,
                                    VendorBusinessNumber = t.VendorBusinessNumber,
                                    VendorBankBsb = t.VendorBankBsb,
                                    VendorBankAccountNumber = t.VendorBankAccountNumber,
                                    BPAYBillerCode = t.BPAYBillerCode,
                                    BPAYReferenceNumber = t.BPAYReferenceNumber,
                                    AccountNumber = t.AccountNumber,
                                    DocDateReceived = t.DocDateReceived,
                                    DocType = t.DocType,
                                    DocLineItemCount = t.DocLineItemCount,
                                    DocReceivedType = t.DocReceivedType,
                                    DocReceivedSource = t.DocReceivedSource,
                                    ExportDate = t.ExportDate,
                                    ProcessStatusCurrent = t.ProcessStatusCurrent,
                                    PaymentTypeCode = t.PaymentTypeCode,
                                    PaymentTermsCode = t.PaymentTermsCode,
                                    Custom01 = t.Custom01,
                                    Custom02 = t.Custom02,
                                    Custom03 = t.Custom03,
                                    Custom04 = t.Custom04,
                                    DocDateDue = t.DocDateDue
                                });
            if (recordID > 0)
            {
                query = query.Where(e => e.ID == recordID);
            }
            if (maxRecords > 0)
            {
                query = query.Take(maxRecords);
            }
            var result = await query.AsNoTracking().ToListAsync().ConfigureAwait(false);

            if (options.LoadGLCodedLines)
            {
                logger.Debug("Attempting to load gl coded lines.");
                foreach (PaymentRequestHeader paymentRequest in result)
                {
                    paymentRequest.SetPaymentRequestGLCodedLines(await GetGLCodedLinesAsync(paymentRequest).ConfigureAwait(false));
                }
            }

            if (options.LoadPOMatchedLines)
            {
                logger.Debug("Attempting to load po matched lines.");
                foreach (PaymentRequestHeader paymentRequest in result)
                {
                    paymentRequest.SetPaymentRequestPOMatchedLines(await GetPOMatchedLinesAsync(paymentRequest).ConfigureAwait(false));
                }
            }

            if (options.LoadSundryPOAllocations)
            {
                logger.Debug("Attempting to load sundry po allocations.");
                foreach (PaymentRequestHeader paymentRequest in result)
                {
                    paymentRequest.SetSundryPOGoodsAllocations(await GetSundryPOAllocationAsync(paymentRequest).ConfigureAwait(false));
                }
            }

            if (options.LoadPOGRAllocations)
            {
                logger.Debug("Attempting to load po gr allocations.");
                foreach (PaymentRequestHeader paymentRequest in result)
                {
                    paymentRequest.SetPurchaseOrderGoodsReceiptAllocation(await GetPurchaseOrderGoodsReceiptAllocationAsync(paymentRequest).ConfigureAwait(false));
                }
            }

            logger.Debug($"Payment request records retrieved. Count: {result.Count:N0}");
            return result;
        }

        public async Task<string> GetFileURL(int recordId)
        {
            /* SSO URL format <host>/login.aspx?q=<Base64 encoded search parameter>.
             * Non-SSO URL format <host>/#weblink?q=<Base 64 encoded search parameter>.
             * 
             * <Base 64 encoded search parameter> = string '0|recordId' encoded with Base64 encoding.
             * 
             * Example: https://think.caaps.com/login.apsx?q=MHw4 or https://think.caaps.com/#weblink?q=MHw4
             * Both of the URLs above point to a CAAPS record with number 8.
             */
            string host = await GetWebServerUrl().ConfigureAwait(false);
            string hostWithSlash = host.EndsWith('/') ? host : host + "/";
            string urlPrefix = await IsSSO().ConfigureAwait(false) ? hostWithSlash + "login.aspx?q=" : hostWithSlash + "#weblink?q=";
            string searchParameter = $"0|{recordId}";
            string encodedSearchParameter = searchParameter.ToBase64();
            string escapedSearchParameter = Uri.EscapeDataString(encodedSearchParameter);
            string result = urlPrefix + escapedSearchParameter;
            logger.Trace(() => $"Generated URL \"{result}\". URL generation details: {nameof(host)}=\"{host}\", {nameof(hostWithSlash)}=\"{hostWithSlash}\", {nameof(urlPrefix)}=\"{urlPrefix}\", {nameof(searchParameter)}=\"{searchParameter}\", {nameof(encodedSearchParameter)}=\"{encodedSearchParameter}\", {nameof(escapedSearchParameter)}=\"{escapedSearchParameter}\".");
            return result;
        }

        public async Task BulkUpdateDocumentStatus(List<PaymentRequestDocumentUpdates> docUpdates)
        {
            if (docUpdates == null) throw new ArgumentNullException(nameof(docUpdates));

            logger.Debug($"Attempting to update multiple documents. Count {docUpdates.Count}");

            var apDocuments = new List<ApDocument>();
            var comments = new List<Comment>();
            var lifeCycleEvents = new List<InvoiceLifeCycleEvent>();
            
            foreach (PaymentRequestDocumentUpdates docUpdate in docUpdates)
            {
                ApDocument apDocument = await Context.ApDocuments.FindAsync(docUpdate.ID);
                apDocument.ProcessStatusCurrent = docUpdate.Status;

                logger.Debug($"Attempting to update document ID: {docUpdate.ID}. ExportDate: {docUpdate.ExportDate}");
                logger.Debug($"Attempting to update document ID: {docUpdate.ID}. ErpImportDate: {docUpdate.ErpImportDate}");

                apDocument.ExportDate = docUpdate.ExportDate;
                apDocuments.Add(apDocument);

                var comment = new Comment
                {
                    ManagedTableID = MANAGEDTABLEID,
                    RecordID = docUpdate.ID,
                    UserName = USERNAME,
                    ActionName = ACTIONNAME,
                    ActionComments = docUpdate.UserComments,
                    RecordStatus = docUpdate.Status,
                    ActionStartDate = docUpdate.ExportDate
                };
                if (!string.IsNullOrWhiteSpace(docUpdate.ExportPayload) || !string.IsNullOrWhiteSpace(docUpdate.ExportResponse))
                {
                    comment.ActionDetail = $"{docUpdate.ExportPayload}\r\n{docUpdate.ExportResponse}".Trim();
                    if (comment.ActionDetail.Length > 1024)
                    {
                        comment.ActionDetail = comment.ActionDetail.Substring(0, 1024);
                    }
                }
                comments.Add(comment);

                lifeCycleEvents.Add(new InvoiceLifeCycleEvent
                {
                    RecordID = docUpdate.ID,
                    ExportDate = docUpdate.ExportDate,
                    ErpImportDate = docUpdate.IsImportConfirmationExpected ? docUpdate.ErpImportDate : (DateTime?)null
                });
            }

            using IDbContextTransaction transaction = BeginTransaction();
            try
            {
                await UpdateManyRecordsAsync(apDocuments).ConfigureAwait(false);
                DbConnection connection = Context.Database.GetDbConnection();
                DbTransaction dbTransaction = Context.Database.CurrentTransaction.GetDbTransaction();
                var commentServices = new CommentServices(Context);
                await commentServices.CreateManyComments(comments, connection, dbTransaction).ConfigureAwait(false);
                var lifeCycleEventServices = new InvoiceLifeCycleEventServices(Context);
                await lifeCycleEventServices.UpdateManyLifeCycleEvent(lifeCycleEvents, connection, dbTransaction).ConfigureAwait(false);
                await Context.SaveChangesAsync().ConfigureAwait(false);
                await CommitTransaction(transaction).ConfigureAwait(false);
            } catch
            {
                await RollbackTransaction(transaction).ConfigureAwait(false);
                throw;
            }
        }

        public async Task<List<PaymentRequestGLCodedLine>> GetGLCodedLinesAsync(PaymentRequestHeader header)
        {
            var result = (from t
                          in Context.AccountCodingLines
                         where t.RecordId == header.ID
                         select new PaymentRequestGLCodedLine
                         {
                             ID = t.ID,
                             CaapsRecordId = header.CaapsRecordId,
                             DocRefNumberA = header.DocRefNumberA,
                             DocDateIssued = header.DocDateIssued,
                             VendorCode = header.VendorCode,
                             DocAmountTotal = header.DocAmountTotal,
                             RecordId = t.RecordId,
                             GLCode = t.GLCode,
                             GLCodeDesc = t.GLCodeDesc,
                             LineDescription = t.LineDescription,
                             LineAmountTaxEx = t.LineAmountTaxEx,
                             LineAmountTax = t.LineAmountTax,
                             LineAmountTotal = t.LineAmountTotal,
                             LineNumber = t.LineNumber,
                             LineTaxCode = t.LineTaxCode,
                             LineCustomFieldA = t.LineCustomFieldA,
                             LineCustomFieldB = t.LineCustomFieldB,
                             LineCustomFieldC = t.LineCustomFieldC,
                             LineCustomFieldD = t.LineCustomFieldD,
                             LineAccountCodeA = t.LineAccountCodeA,
                             LineAccountCodeB = t.LineAccountCodeB,
                             LineAccountCodeC = t.LineAccountCodeC,
                             LineAccountCodeD = t.LineAccountCodeD,
                             LineAccountCodeE = t.LineAccountCodeE,
                             LineAccountCodeF = t.LineAccountCodeF,
                             LineAccountType = t.LineAccountType,
                             LineApprovedYN = t.LineApprovedYN,
                             LineCalcPercent = t.LineCalcPercent,
                             LineVACDesc = t.LineVACDesc,
                             LineVACLineNumber = t.LineVACLineNumber
                         });

            return await result.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<PaymentRequestPOMatchedLine>> GetPOMatchedLinesAsync(PaymentRequestHeader header)
        {
            var result = from t
                          in Context.LineItems
                         where t.RecordId == header.ID
                         select new PaymentRequestPOMatchedLine
                         {
                             ID = t.ID,
                             RecordId = header.ID,
                             PoNumber = header.PoNumber,
                             DocRefNumberA = header.DocRefNumberA,
                             DocDateIssued = header.DocDateIssued,
                             VendorCode = header.VendorCode,
                             DocAmountTotal = header.DocAmountTotal,
                             CaapsRecordId = header.CaapsRecordId,
                             LineNumber = t.LineNumber,
                             LineQuantity = t.LineQuantity,
                             LineUOM = t.LineUOM,
                             LineAmountTax = t.LineAmountTax,
                             LineAmountTaxEx = t.LineAmountTaxEx,
                             LineAmountTotal = t.LineAmountTotal,
                             LineOriginalAmountTotal = t.LineOriginalAmountTotal,
                             LineUnitAmountTaxEx = t.LineUnitAmountTaxEx,
                             LinePoNumber = t.LinePoNumber,
                             LinePoLineNumber = t.LinePoLineNumber,
                             LineProductCode = t.LineProductCode,
                             LineTaxCode = t.LineTaxCode,
                             LineDescription = t.LineDescription,
                             LineValidAdditionalCharge = t.LineValidAdditionalCharge,
                             LineOriginalPoNumber = t.LineOriginalPoNumber,
                             LineOriginalProductCode = t.LineOriginalProductCode,
                             OriginalUnitAmount = t.OriginalUnitAmount,
                             PoIssuedBy = t.PoIssuedBy,
                             PoType = t.PoType
                         };
            var materializedResult = await result.AsNoTracking().ToListAsync().ConfigureAwait(false);
            foreach (PaymentRequestPOMatchedLine poMatchedLine in materializedResult)
            {
                poMatchedLine.AllocatedPurchaseOrder = await Context.PurchaseOrders.Where(t => t.PoNumber == poMatchedLine.PoNumber &&
                                                                                               t.LineNumber == poMatchedLine.LinePoLineNumber &&
                                                                                               t.VendorCode == poMatchedLine.VendorCode).FirstOrDefaultAsync().ConfigureAwait(false);

                if (poMatchedLine.AllocatedPurchaseOrder != null)
                {
                    poMatchedLine.AllocatedPurchaseOrder.SetAllocatedGoodsReceipts(await Context.GoodsReceipts.Where(t => t.PoNumber == poMatchedLine.AllocatedPurchaseOrder.PoNumber &&
                                                                                                                          t.PoLineNumber == poMatchedLine.AllocatedPurchaseOrder.LineNumber).ToListAsync().ConfigureAwait(false));
                }
            }

            return materializedResult;
        }

        public async Task<List<SundryPOGoodsAllocation>> GetSundryPOAllocationAsync(PaymentRequestHeader header)
        {
            return await Context.SundryPOGoodsAllocations
                                .Where(t => t.RecordID == header.ID)
                                .AsNoTracking()
                                .ToListAsync()
                                .ConfigureAwait(false);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrderGoodsReceiptAllocationAsync(PaymentRequestHeader header)
        {
            return await Context.PurchaseOrders
                                .Where(p => p.VendorCode == header.VendorCode && p.PoNumber == header.PoNumber)
                                .Include(p => p.GoodsReceipts)
                                .AsNoTracking()
                                .ToListAsync()
                                .ConfigureAwait(false);
        }
    }
}