using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.ErpPaymentRequest.Common;
using AD.CAAPS.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase - N/A for JSON lower-case name resolver
            return name?.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
        }
    }

    public class UrbanisePaymentRequest
    {
        private readonly AppSettings appSettings;
        private readonly PaymentRequestServices paymentRequestServices;
        private readonly VendorServices vendorServices;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UrbanisePaymentRequest(DBConfiguration dbConfiguration, AppSettings appSettings)
        {
            if (dbConfiguration is null)
            {
                throw new ArgumentNullException(nameof(dbConfiguration));
            }
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            paymentRequestServices = new PaymentRequestServices(dbConfiguration, appSettings.PaymentRequestOptions);
            vendorServices = new VendorServices(dbConfiguration);
        }

        private async Task<string> DownloadCAAPSApiPdfDocument(string tempDownloadPath, int ID, string CaapsRecordId)
        {
            string downloadedFile = Path.Combine(tempDownloadPath, CaapsRecordId + CAAPSConstants.PDF_EXTENSION);
            var docDownloadRequestUrl = string.Format(appSettings.CaapsApiDownloadInvoiceDocumentEndpoint, ID);
            var docDownloadRequest = RestSharpUtils.CreateRequestForDownloadPDFFromADAzurePortal(appSettings.XClientId, appSettings.OcpApimTrace, appSettings.OcpApimSubscriptionKey);
            var docDownloadUtils = new RestSharpUtils(new Uri(docDownloadRequestUrl), docDownloadRequest);
            await docDownloadUtils.DownloadToFilePath(downloadedFile).ConfigureAwait(false);
            long fileSize = FileUtils.GetFileSize(downloadedFile);
            logger.Trace($"Downloaded PDF document for CAAPS ID: {ID}: \"{downloadedFile}\". Bytes: {fileSize:N0}");
            return downloadedFile;
        }

        // Pay Types:
        enum UrbanisePayType
        {
            CHEQUE = 0,
            DIRECT_DEBIT = 1,
            EFT = 2,
            BPAY = 4
        }

        const int UrbanisePaymentRequestPutStatusCode = 3; // 3: Review

        private async Task<ExportData> SetExtendedExportData(PaymentRequestHeader paymentRequest, ExportData exportData)
        {
            Vendor vendor = null;
            if (exportData.SupplierId != null && exportData.SupplierId > 0)
            {
                vendor = await vendorServices.GetVendorByVendorCode(exportData.SupplierId.ToString()).ConfigureAwait(false);
                if (vendor != null)
                {
                    exportData.SupplierName = string.IsNullOrWhiteSpace(vendor.VendorName) ? "" : vendor.VendorName;
                }
            }
            if (!string.IsNullOrWhiteSpace(paymentRequest.PaymentTypeCode))
            {
                if (paymentRequest.PaymentTypeCode.Equals("BPAY", StringComparison.InvariantCultureIgnoreCase))
                {
                    exportData.PayType = (int)UrbanisePayType.BPAY;
                    exportData.BpayReference = paymentRequest.BPAYReferenceNumber;
                }
                else if (paymentRequest.PaymentTypeCode.Equals("EFT", StringComparison.InvariantCultureIgnoreCase))
                {
                    exportData.PayType = (int)UrbanisePayType.EFT;
                    exportData.Reference = paymentRequest.DocRefNumberA;
                }
                else if (paymentRequest.PaymentTypeCode.Equals("DIRECT DEBIT", StringComparison.InvariantCultureIgnoreCase))
                {
                    exportData.PayType = (int)UrbanisePayType.DIRECT_DEBIT;
                }
                else if (paymentRequest.PaymentTypeCode.Equals("CHEQUE", StringComparison.InvariantCultureIgnoreCase))
                {
                    exportData.PayType = (int)UrbanisePayType.CHEQUE;
                };
            }
            if (vendor != null)
            {
                if (!string.IsNullOrWhiteSpace(vendor.PaymentTypeCode) && vendor.PaymentTypeCode.Equals("EFT", StringComparison.InvariantCultureIgnoreCase))
                {
                    exportData.Note = "Captured BSB: " + paymentRequest.VendorBankBsb + ", BankAccountNumber: " + paymentRequest.VendorBankAccountNumber;
                }
                else
                {
                    exportData.Note = "Captured BPAY Biller Code = " + paymentRequest.BPAYBillerCode + ", ReferenceNumber: " + paymentRequest.BPAYReferenceNumber;
                }
            }
            else
            {
                exportData.SupplierId = 0;
                exportData.SupplierName = "";
            }
            if (exportData.PropertyId == null) exportData.PropertyId = 0;
            if (string.IsNullOrWhiteSpace(exportData.Note)) exportData.Note = "";
            if (string.IsNullOrWhiteSpace(exportData.InvoiceNo)) exportData.InvoiceNo = "";
            if (string.IsNullOrWhiteSpace(exportData.Reference)) exportData.Reference = "";
            if (exportData.Status == null) exportData.Status = UrbanisePaymentRequestPutStatusCode;
            return exportData;
        }

        private async Task<PaymentRequestDocumentUpdates> ExportSinglePaymentRequest(string progress, string tempDownloadPath,
            PaymentRequestHeader paymentRequest, bool dryRun)
        {
            string downloadedFile = "";
            try
            {
                downloadedFile = await DownloadCAAPSApiPdfDocument(tempDownloadPath, paymentRequest.ID, paymentRequest.CaapsRecordId).ConfigureAwait(false);
                // files.Add(downloadedFile);
                logger.Trace($"Downloaded PDF document: {progress}: {downloadedFile}");
                if (string.IsNullOrWhiteSpace(downloadedFile) || !File.Exists(downloadedFile))
                {
                    throw new ErpExportFailedException("Could not download the PDF document from the CAAPS API");
                }
                string pdfUploadEndpoint = string.IsNullOrWhiteSpace(paymentRequest.BranchCode) ?
                                        string.Format(appSettings.PDFUploadEndpoint, paymentRequest.EntityCode) :
                                        string.Format(appSettings.PDFUploadEndpointPropertyIdRequired, paymentRequest.EntityCode, paymentRequest.BranchCode);
                RestSharp.IRestResponse pdfUploadResponse = null;
                RestSharp.RestRequest pdfUploadRequest = RestSharpUtils.CreateRequestForUploadPDF(downloadedFile);
                var pdfUploadRequestUtils = new RestSharpUtils(new Uri(pdfUploadEndpoint), pdfUploadRequest);

                if (!string.IsNullOrWhiteSpace(appSettings.ClientBearerToken))
                {
                    pdfUploadRequest.AddHeader("Authorization", "Bearer " + appSettings.ClientBearerToken);
                }
                /*if (appSettings.ClientSupportsCompression)
                {
                    logger.Trace("Requesting response compression");
                    pdfUploadRequest.AddHeader("Accept-Encoding", "gzip");
                }*/

#if URBANISE_ENDPOINT_REQUESTS_ENABLED
                if (!dryRun)
                {
                    pdfUploadResponse = await pdfUploadRequestUtils.SendRequestAsync().ConfigureAwait(false);
                }
#endif
                logger.Debug($"Request {progress} on {pdfUploadEndpoint} returned response {pdfUploadResponse?.StatusCode}");

                var erpJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                string pdfUploadResponseContent = null;
                if (!dryRun)
                {
                    if (pdfUploadResponse == null)
                        throw new ErpExportFailedException("Payment request failed to upload PDF document with no response returned");

                    if (!RestSharpUtils.IsResposeSuccess(pdfUploadResponse))
                    {
                        string message = $"Request failed while sending payment request export for document with id {paymentRequest.ID}. Response: {RestSharpUtils.GetRestResponseErrorDetails(pdfUploadResponse)}";
                        logger.Error(message);
                        return new PaymentRequestDocumentUpdates
                        {
                            ID = paymentRequest.ID,
                            Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                            IsImportConfirmationExpected = appSettings.IsImportConfirmationExpected,
                            UserComments = message,
                            FileName = downloadedFile
                        };
                    }
                    else if (string.IsNullOrWhiteSpace(pdfUploadResponse.Content))
                    {
                        string message = $"Request failed while sending payment request export for document with id {paymentRequest.ID}. No response body.";
                        logger.Error(message);
                        return new PaymentRequestDocumentUpdates
                        {
                            ID = paymentRequest.ID,
                            Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                            IsImportConfirmationExpected = appSettings.IsImportConfirmationExpected,
                            UserComments = message,
                            FileName = downloadedFile
                        };
                    }
                    else
                    {
                        pdfUploadResponseContent = pdfUploadResponse.Content;
                    }
                }
                else
                {
                    pdfUploadResponseContent = JsonConvert.SerializeObject(new
                    {
                        Id = 0,
                        PropertyId = 0,
                        Status = 0,
                        Amount = 0.0,
                        BpayReference = "",
                        DueDate = DateTime.Now.Date,
                        InvoiceDate = DateTime.Now.Date,
                        InvoiceNo = "",
                        Note = "",
                        PayType = 0,
                        Reference = "",
                        SupplierId = 0,
                        SupplierName = ""
                    }, erpJsonSettings);
                };
                ExportData exportData;
                if (Utils.IsStringJSONObject(pdfUploadResponseContent))
                {
                    exportData = JsonConvert.DeserializeObject<ExportData>(pdfUploadResponseContent, erpJsonSettings);
                    if (exportData == null) throw new JsonSerializationException("PDF upload JSON deserialized object is null");
                    exportData.InvoiceDate = Utils.ConvertDateTimeToString(paymentRequest.DocDateIssued, appSettings.ExportDateFormat, true);
                    exportData.InvoiceNo = paymentRequest.DocRefNumberA;
                    exportData.DueDate = Utils.ConvertDateTimeToString(paymentRequest.DocDateDue, appSettings.ExportDateFormat, true);
                    if (int.TryParse(paymentRequest.VendorCode, out int SupplierId))
                    {
                        exportData.SupplierId = SupplierId;
                    }
                    // Invoice Reference - reference number used to reconcile payment, should be on the invoice
                    exportData.Reference = paymentRequest.AccountNumber;
                    exportData.Amount = paymentRequest.DocAmountTotal;
                    exportData = await SetExtendedExportData(paymentRequest, exportData).ConfigureAwait(false);
                }
                else
                {
                    throw new JsonSerializationException($"PDF upload server response is not a valid JSON object: \r\n{pdfUploadResponse.Content}");
                }

                string dataExportEndpoint = string.Format(appSettings.PaymentRequestExportEndpoint, paymentRequest.EntityCode, exportData.Id);
                RestSharp.RestRequest dataExportRequest = RestSharpUtils.CreateRequestForPut();
                dataExportRequest.AddHeader("Content-Type", "application/json");

                string exportDataJson = JsonConvert.SerializeObject(exportData, erpJsonSettings);
                logger.Trace(() => $"ExportData: ------\r\n{exportDataJson}\r\n-----\r\n");

                if (!string.IsNullOrWhiteSpace(appSettings.ClientBearerToken))
                {
                    dataExportRequest.AddHeader("Authorization", "Bearer " + appSettings.ClientBearerToken);
                }
                dataExportRequest.AddParameter("application/json", exportDataJson, RestSharp.ParameterType.RequestBody);
                RestSharpUtils dataExportRequestUtils = new RestSharpUtils(new Uri(dataExportEndpoint), dataExportRequest);
                logger.Debug(() => $"Attempting to send request {progress} to {dataExportEndpoint} with body {exportDataJson}");

                RestSharp.IRestResponse dataExportResponse = null;
#if URBANISE_ENDPOINT_REQUESTS_ENABLED
                if (!dryRun)
                {
                    dataExportResponse = await dataExportRequestUtils.SendRequestAsync().ConfigureAwait(false);
                }
                else
                {

                }
#endif
                logger.Debug($"Request {progress} on {dataExportEndpoint} returned response {dataExportResponse?.StatusCode}");

                if (dataExportResponse == null)
                    throw new ErpExportFailedException("Payment requet failed with no response returned");

                if (!RestSharpUtils.IsResposeSuccess(dataExportResponse))
                {
                    string errorDetails = RestSharpUtils.GetRestResponseErrorDetails(dataExportResponse);
                    string message = $"Payment Request failed to send to ERP for id {paymentRequest.ID}. \r\nRequest:\r\n{exportDataJson}\r\nResponse: \r\n{errorDetails}\r\n";
                    logger.Error(message);
                    return new PaymentRequestDocumentUpdates
                    {
                        ID = paymentRequest.ID,
                        Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                        IsImportConfirmationExpected = appSettings.IsImportConfirmationExpected,
                        UserComments = $"Payment Request failed to export. ERP response: {errorDetails}",
                        FileName = downloadedFile,
                        ExportPayload = exportDataJson,
                        ExportResponse = errorDetails
                    };
                }
                else
                {
                    string responseDetails = dataExportResponse.Content;
                    string message = $"Payment request export successful for document with id {paymentRequest.ID}.\r\nRequest:\r\n{exportDataJson}\r\nResponse: \r\n{responseDetails}";
                    logger.Info(message);
                    string responseNote = responseDetails;
                    if (!string.IsNullOrWhiteSpace(responseDetails))
                    {
                        //Response: { "id":2,"propertyId":"40877","note":"\nThe Payment Method EFT is not valid for this supplier or property.\nThe Reference was not provided.","supplierId":1374619,"amount":5821.53,"invoiceDate":"2019-10-02","invoiceNo":"00046612","reference":"","dueDate":"2020-06-05","status":1,"supplierName":"Inside Outside Facility Services","payType":0,"bpayReference":null}
                        JObject responseObject = JObject.Parse(responseDetails);
                        if (responseObject.TryGetValue("note", out JToken note))
                        {
                            responseNote = note.ToString();
                        }
                    }
                    string documentComment = string.IsNullOrWhiteSpace(responseDetails) ?
                            "Payment Request exported" :
                            $"Payment Request exported. Response: {responseNote}";
                    return new PaymentRequestDocumentUpdates
                    {
                        ID = paymentRequest.ID,
                        Status = appSettings.DocumentStatusAfterExtract,
                        IsImportConfirmationExpected = false,
                        UserComments = documentComment,
                        FileName = downloadedFile,
                        ExportPayload = exportDataJson,
                        Success = true
                    };
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types. Catching and logging all errors here.
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                logger.Error(e, $"Error processing payment requests after {progress}");
                string message = $"An exception caught while sending payment request export for document with id {paymentRequest.ID}. Exception Message: {e.Message}";
                logger.Error(e, message);
                return new PaymentRequestDocumentUpdates
                {
                    ID = paymentRequest.ID,
                    Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                    IsImportConfirmationExpected = false,
                    UserComments = message,
                    FileName = downloadedFile
                };
            }
        }

        public async Task<int> Export(int recordID, int maxRecords, bool dryRun)
        {
            if (string.IsNullOrWhiteSpace(appSettings.CaapsApiDownloadInvoiceDocumentEndpoint)) throw new ConfigurationMissingException("AppSettings:CaapsApiDownloadInvoiceDocumentEndpoint not configured");
            if (string.IsNullOrWhiteSpace(appSettings.PDFUploadEndpoint)) throw new ConfigurationMissingException("AppSettings:PDFUploadEndpoint not configured");
            if (string.IsNullOrWhiteSpace(appSettings.PDFUploadEndpointPropertyIdRequired)) throw new ConfigurationMissingException("AppSettings:PDFUploadEndpointPropertyIdRequired not configured");
            if (string.IsNullOrWhiteSpace(appSettings.PaymentRequestExportEndpoint)) throw new ConfigurationMissingException("AppSettings:PaymentRequestExportEndpoint not configured");
            if (string.IsNullOrWhiteSpace(appSettings.ClientBearerToken)) throw new ConfigurationMissingException("AppSettings:ClientBearerToken not configured");
            if (string.IsNullOrWhiteSpace(appSettings.ExportDateFormat)) throw new ConfigurationMissingException("AppSettings:ExportDateFormat not configured");

            logger.Debug($"Starting to process payment request export. MaxRecords: {maxRecords:N0}. DryRun: {dryRun}");
            IList<PaymentRequestHeader> paymentRequests = await paymentRequestServices.GetPaymentRequests(recordID, maxRecords).ConfigureAwait(false);
            string tempDownloadPath = FileUtils.CheckPathNotExistThenCreate(appSettings.TempDownloadPath);
            var docUpdates = new List<PaymentRequestDocumentUpdates>();
            int count = 0;
            int success = 0;
            int failed = 0;
            List<string> files = new List<string>();
            try
            {
                logger.Debug($"Payment requests ready for export: {paymentRequests.Count:N0}");
                foreach (PaymentRequestHeader paymentRequest in paymentRequests)
                {
                    count++;
                    string progress = $"ID: {paymentRequest.ID} ({count:N0} of {paymentRequests.Count:N0} total)";
                    logger.Trace($"Payment requests exporting: {progress}");
                    var update = await ExportSinglePaymentRequest(progress, tempDownloadPath, paymentRequest, dryRun).ConfigureAwait(false);
                    if (update != null)
                    {
                        if (!dryRun)
                        {
                            docUpdates.Add(update);
                        }
                        if (!string.IsNullOrWhiteSpace(update.FileName))
                        {
                            files.Add(update.FileName);
                        }
                        if (update.Success)
                        {
                            success++;
                        }
                    }
                }

                if (!dryRun)
                {
                    await paymentRequestServices.BulkUpdateDocumentStatus(docUpdates).ConfigureAwait(false);
                }
                failed = paymentRequests.Count - success;
                logger.Debug($"Successfully exported: {success}/{paymentRequests.Count}");
                logger.Debug($"Failed to export: {failed}/{paymentRequests.Count}");
            }
            finally
            {
                files.ForEach(async fn =>
                {
                    try
                    {
                        await FileUtils.DeleteFile(fn).ConfigureAwait(false);
                    }
                    catch (IOException ex)
                    {
                        logger.Error(ex, "Failed to delete files: " + ex.Message);
                    }
                });
            }
            
            if (paymentRequests.Count < 1)
            {
                logger.Warn("No payment requests ready for export");
                return (int)ExitCode.CompletedButWithUnprocessed;
            }
            if (failed > 0)
            {
                logger.Warn("Some payment requests failed to export");
                return (int)ExitCode.CompletedButWithErrors;
            }

            logger.Info($"Payment requests successfully exported: {paymentRequests.Count:N0}");
            return (int)ExitCode.Successful;
            // logger.Debug($"Program has exited with code {Utils.GetEnvironmentExitCode()}");
        }
    }
}