using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using AD.CAAPS.ErpPaymentRequest;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AD.CAAPS.ErpPaymentRequest.Common;

namespace AD.CAAPS.ErpPaymentRequest.Porirua
{
    public class PaymentRequest
    {
        private readonly AppSettings appSettings;
        private readonly PaymentRequestServices paymentRequestServices;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public PaymentRequest(DBConfiguration dbConfiguration, AppSettings appSettings)
        {
            if (dbConfiguration is null)
            {
                throw new ArgumentNullException(nameof(dbConfiguration));
            }
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            paymentRequestServices = new PaymentRequestServices(dbConfiguration, appSettings.PaymentRequestOptions);
        }

        public async Task<ExitCode> Export()
        {
            logger.Debug("Starting to process payment request export");
            ExitCode exitCode = ExitCode.Successful;
            
            IList<PaymentRequestHeader> paymentRequests = await paymentRequestServices.GetPaymentRequests();
            
            string tempDownloadPath = FileUtils.CheckPathNotExistThenCreate(appSettings.TempDownloadPath);
            var docUpdates = new List<PaymentRequestDocumentUpdates>();

            RestSharp.RestClient client1 = RestSharpUtils.ClientWithBasicAuthentication(new Uri(appSettings.PaymentRequestExportEndpoint), appSettings.Username, appSettings.Password);

            int success = 0;
            foreach (PaymentRequestHeader paymentRequest in paymentRequests)
            {
                try
                {
                    var exportData = new ExportData
                    {
                        CreditorNumber = paymentRequest.VendorCode,
                        CaapsID = paymentRequest.CaapsRecordId,
                        GstAmount = paymentRequest.DocAmountTax,
                        InvoiceTotalAmount = paymentRequest.DocAmountTotal,
                        PoNumber = paymentRequest.PoNumber,
                        InvoiceDate = Utils.ConvertDateTimeToString(paymentRequest.DocDateIssued, appSettings.DateFormat),
                        InvoiceDueDate = Utils.ConvertDateTimeToString(paymentRequest.DocDateDue, appSettings.DateFormat),
                        InvoiceNumber = paymentRequest.DocRefNumberA
                    };

                    RestSharp.RestRequest erpPostRequest = RestSharpUtils.CreateRequestForPostJson();
                    erpPostRequest.AddParameter("application/json", JsonConvert.SerializeObject(exportData), RestSharp.ParameterType.RequestBody);
                    
                    var erpPostUtils = new RestSharpUtils(new Uri(appSettings.PaymentRequestExportEndpoint), erpPostRequest);
                    RestSharp.IRestResponse response1 = await erpPostUtils.SendRequestAsync(client1);
                    logger.Debug($"Request on {client1.BaseUrl} returned response {response1.StatusCode}");

                    if (!RestSharpUtils.IsResposeSuccess(response1))
                    {
                        string message = $"Request failed while sending payment request export for document with id {paymentRequest.ID}. Response: {RestSharpUtils.GetRestResponseErrorDetails(response1)}";
                        logger.Error(message);
                        docUpdates.Add(new PaymentRequestDocumentUpdates
                        {
                            ID = paymentRequest.ID,
                            Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                            IsImportConfirmationExpected = appSettings.IsImportConfirmationExpected,
                            UserComments = message
                        });
                    }
                    else if (string.IsNullOrWhiteSpace(response1.Content))
                    {
                        string message = $"Request failed while sending payment request export for document with id {paymentRequest.ID}. No response found.";
                        logger.Error(message);
                        docUpdates.Add(new PaymentRequestDocumentUpdates
                        {
                            ID = paymentRequest.ID,
                            Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                            IsImportConfirmationExpected = appSettings.IsImportConfirmationExpected,
                            UserComments = message
                        });
                    }
                    else
                    {
                        string downloadedFile = Path.Combine(tempDownloadPath, paymentRequest.CaapsRecordId + CAAPSConstants.PDF_EXTENSION);

                        var docDownloadRequestUrl = string.Format(appSettings.CaapsApiDownloadInvoiceDocumentEndpoint, paymentRequest.ID);
                        var docDownloadRequest = RestSharpUtils.CreateRequestForDownloadPDFFromADAzurePortal(appSettings.XClientId, appSettings.OcpApimTrace, appSettings.OcpApimSubscriptionKey);
                        var docDownloadUtils = new RestSharpUtils(new Uri(docDownloadRequestUrl), docDownloadRequest);

                        await docDownloadUtils.DownloadToFilePath(downloadedFile);

                        string endpoint2 = string.Format(appSettings.PDFUploadEndpoint, response1.Content);

                        RestSharp.RestClient client2 = RestSharpUtils.ClientWithBasicAuthentication(new Uri(endpoint2), appSettings.Username, appSettings.Password);
                        RestSharp.RestRequest docUploadRequest = RestSharpUtils.CreateRequestForUploadPDF(downloadedFile);
                        RestSharpUtils docUploadUtils = new RestSharpUtils(new Uri(endpoint2), docUploadRequest);
                        RestSharp.IRestResponse response2 = await docUploadUtils.SendRequestAsync(client2);
                        await FileUtils.DeleteFile(downloadedFile);
                        logger.Debug($"Request on {client2.BaseUrl} returned response {response2.StatusCode}");

                        if (!RestSharpUtils.IsResposeSuccess(response2))
                        {
                            string message = $"Request failed while uploading file for document with id {paymentRequest.ID}. Response: {RestSharpUtils.GetRestResponseErrorDetails(response2)}";
                            logger.Error(message);
                            docUpdates.Add(new PaymentRequestDocumentUpdates
                            {
                                ID = paymentRequest.ID,
                                Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                                IsImportConfirmationExpected = appSettings.IsImportConfirmationExpected,
                                UserComments = message
                            });
                        }
                        else
                        {
                            string endpoint3 = string.Format(appSettings.PaymentRequestCompletionEndpoint, response1.Content);
                            RestSharp.RestClient client3 = RestSharpUtils.ClientWithBasicAuthentication(new Uri(endpoint3), appSettings.Username, appSettings.Password);

                            RestSharp.RestRequest erpPutDataRequest = RestSharpUtils.CreateRequestForPut();
                            RestSharpUtils erpPutDataUtils = new RestSharpUtils(new Uri(endpoint3), erpPutDataRequest);
                            RestSharp.IRestResponse response3 = await erpPutDataUtils.SendRequestAsync(client3);
                            logger.Debug($"Request on {client3.BaseUrl} returned response {response3.StatusCode}");

                            if (!RestSharpUtils.IsResposeSuccess(response3))
                            {
                                string message = $"Request failed while completing the payment request for document with id {paymentRequest.ID}. Response: {RestSharpUtils.GetRestResponseErrorDetails(response3)}";
                                logger.Error(message);
                                docUpdates.Add(new PaymentRequestDocumentUpdates
                                {
                                    ID = paymentRequest.ID,
                                    Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                                    IsImportConfirmationExpected = false,
                                    UserComments = message
                                });
                            }
                            else
                            {
                                string message = $"Payment request export successful for document with id {paymentRequest.ID}";
                                logger.Debug(message);
                                docUpdates.Add(new PaymentRequestDocumentUpdates
                                {
                                    ID = paymentRequest.ID,
                                    Status = appSettings.DocumentStatusAfterExtract,
                                    IsImportConfirmationExpected = false,
                                    UserComments = message
                                });
                                success++;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string message = $"An exception caught while sending payment request export for document with id {paymentRequest.ID}. Exception Message: {e.Message}";
                    logger.Error(e, message);
                    docUpdates.Add(new PaymentRequestDocumentUpdates
                    {
                        ID = paymentRequest.ID,
                        Status = CAAPSConstants.PROCESS_STATUS_CURRENT_EXPORT_FAILED,
                        IsImportConfirmationExpected = false,
                        UserComments = message
                    });
                }
            }

            await paymentRequestServices.BulkUpdateDocumentStatus(docUpdates);
            int failed = paymentRequests.Count - success;
            logger.Debug($"Successfully exported: {success}/{paymentRequests.Count}");
            logger.Debug($"Failed to export: {failed}/{paymentRequests.Count}");
            
            if (paymentRequests.Count < 1)
                exitCode = ExitCode.CompletedButWithUnprocessed;
            if (failed > 0)
                exitCode = ExitCode.CompletedButWithErrors;

            logger.Debug($"Program has exited with code {exitCode}");

            return exitCode;
        }
    }
}