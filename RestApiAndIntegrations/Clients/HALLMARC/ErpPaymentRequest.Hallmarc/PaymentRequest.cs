using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using AD.CAAPS.ErpPaymentRequest;
using AD.CAAPS.ErpPaymentRequest.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AD.CAAPS.ErpPaymentRequest.Hallmarc
{
    public class PaymentRequest
    {
        private readonly AppSettings appSettings;
        private readonly PaymentRequestServices paymentRequestServices;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        const string DEFAULT_FILE_NAME = "file", DEFAULT_FILE_EXT = ".txt";
        const char DEFAULT_FIELD_SEPARATOR = ',';

        public PaymentRequest(DBConfiguration dbConfiguration, 
                     AppSettings appSettings)
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
            logger.Debug("Started running ExtractPaymentRequestExport()");
            ExitCode exitCode = ExitCode.Successful;

            IList<PaymentRequestHeader> paymentRequests = await paymentRequestServices.GetPaymentRequests();
            
            foreach (Dictionary<string, string> dictionary in appSettings.EntityGroups)
            {
                string custom01 = "", custom02 = "";
                foreach (var key in dictionary.Keys)
                {
                    if (key == "CUSTOM_01")
                        custom01 = dictionary[key];
                    else
                        custom02 = dictionary[key];
                }

                var grouped = (from t
                                in paymentRequests
                                where t.Custom01.Equals(custom01, StringComparison.OrdinalIgnoreCase) &&
                                      t.Custom02.Equals(custom02, StringComparison.OrdinalIgnoreCase)
                                select t).ToList();

                if (grouped.Count > 0)
                {
                    string fileName = BuildFileName(custom01, custom02, DateTime.UtcNow.ToString("yyyyMMdd hhmmss"));
                    try
                    {
                        await ExtractGroupToFileAsync(grouped, fileName);

                        var docUpdates = new List<PaymentRequestDocumentUpdates>();

                        foreach (PaymentRequestHeader header in grouped)
                        {
                            docUpdates.Add(
                                new PaymentRequestDocumentUpdates
                                {
                                    ID = header.ID,
                                    Status = appSettings.DocumentStatusAfterExtract,
                                    ExportDate = DateTime.UtcNow,
                                    IsImportConfirmationExpected = false,
                                    UserComments = "Payment request export succesful"
                                });
                        }
                        
                        await paymentRequestServices.BulkUpdateDocumentStatus(docUpdates);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e, $"Failed to create file {fileName}. Message: {e.Message}");
                        exitCode = ExitCode.CompletedButWithErrors;
                    }
                }
                else
                {
                    logger.Debug($"No data found for group {custom01}-{custom02}");
                    exitCode = ExitCode.CompletedButWithUnprocessed;
                }
            }

            return exitCode;
        }

        public async Task ExtractGroupToFileAsync(List<PaymentRequestHeader> paymentRequests, string fileName)
        {
            if (paymentRequests is null)
            {
                throw new ArgumentNullException(nameof(paymentRequests));
            }

            char fieldSeparator = Utils.ParseCharSetDefault(appSettings.FileSettings["FieldSeparator"],  DEFAULT_FIELD_SEPARATOR);

            logger.Debug($"Creating file {fileName}");
            var builder = new StringBuilder();
            foreach (PaymentRequestHeader header in paymentRequests)
            {
                var headerString = string.Join(fieldSeparator,
                                                "API",
                                                header.VendorCode,
                                                header.DocRefNumberA,
                                                header.DocDescription,
                                                header.DocAmountTotal,
                                                header.DocAmountTax,
                                                null,
                                                null,
                                                Utils.ConvertDateTimeToString(header.DocDateIssued, appSettings.ExportFileDateFieldsFormat),
                                                Utils.ConvertDateTimeToString(header.DocDateReceived, appSettings.ExportFileDateFieldsFormat),
                                                null,
                                                null,
                                                Utils.ConvertDateTimeToString(header.DocDateDue, appSettings.ExportFileDateFieldsFormat),
                                                header.CaapsRecordId,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null,
                                                null);
                builder.AppendLine(headerString);

                if (appSettings.PaymentRequestOptions.LoadGLCodedLines)
                {
                    foreach (PaymentRequestGLCodedLine lineItem in header.PaymentRequestGLCodedLines)
                    {
                        var lineItemString = string.Join(fieldSeparator,
                                                        "APD",
                                                        null,
                                                        null,
                                                        lineItem.LineAccountCodeA,
                                                        lineItem.LineAccountCodeB,
                                                        lineItem.LineAccountCodeC,
                                                        BuildAccountsPayableAccount(lineItem.LineAccountCodeC),
                                                        null,
                                                        lineItem.LineTaxCode,
                                                        lineItem.LineAmountTotal,
                                                        lineItem.LineAmountTax,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        Utils.StringToCSVCell(lineItem.LineDescription),
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null);

                        builder.AppendLine(lineItemString);
                    }
                }
            }

            string fullPath;
            if (appSettings.SaveToFolder)
            {
                fullPath = Path.Combine(appSettings.OutputFileLocation, fileName);
            }
            else
            {
                fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            }
            using (var file = new StreamWriter(fullPath))
            {
                await file.WriteAsync(builder.ToString());
            }
            logger.Debug($"File has been written to {fullPath}");

            if (appSettings.UploadToFTP)
            {
                try
                {
                    await UploadFileToFTPAsync(fullPath);
                }
                finally
                {
                    await DeleteTemporaryFile(fullPath);
                }
            }
            else
            {
                await DeleteTemporaryFile(fullPath).ConfigureAwait(false);
            }
        }

        private async Task DeleteTemporaryFile(string fileName)
        {
            if (!appSettings.SaveToFolder)
            {
                await FileUtils.DeleteFile(fileName).ConfigureAwait(false);
            }
        }

        private string BuildFileName(params string[] appendToFileName)
        {
            string filename = Utils.StringFirstNotNull(appSettings.FileSettings["Filename"], DEFAULT_FILE_NAME);
            string fileext = Utils.StringFirstNotNull(appSettings.FileSettings["Fileext"], DEFAULT_FILE_EXT);

            var appendString = string.Join('_', appendToFileName);
            appendString = string.Join('_', filename, appendString);
            return string.Concat(appendString, fileext);
        }

        private string BuildAccountsPayableAccount(string entityCode)
        {
            if (string.IsNullOrWhiteSpace(entityCode))
                return null;

            entityCode = entityCode.Substring(0, 4);
            return string.Format(appSettings.AccountsPayableAccountFormat, entityCode);
        }

        private async Task UploadFileToFTPAsync(string filePath)
        {
            string hostname = Uri.EscapeUriString(appSettings.FTPSettings["Hostname"]),
                   username = appSettings.FTPSettings["Username"],
                   password = appSettings.FTPSettings["Password"];
            await FTPUtils.FTPUploadAsync(hostname, username, password, filePath);
        }
    }
}