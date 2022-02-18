using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using AD.CAAPS.ErpPaymentRequest.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AD.CAAPS.ErpPaymentRequest.Elanor
{
    public class PaymentRequest
    {
        private readonly AppSettings appSettings;
        private readonly PaymentRequestServices paymentRequestServices;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        const string DEFAULT_FILE_NAME = "file", DEFAULT_FILE_EXT = ".txt", DEFAULT_DATE_FORMAT = "yyyyMMdd";
        const char DEFAULT_FIELD_SEPARATOR = ',';

        public PaymentRequest(DBConfiguration dbConfiguration, 
                     AppSettings appSettings)
        {
            if (dbConfiguration is null) throw new ArgumentNullException(nameof(dbConfiguration));
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            paymentRequestServices = new PaymentRequestServices(dbConfiguration, appSettings.PaymentRequestOptions);
        }

        public async Task<ExitCode> Export()
        {
            logger.Debug("Started running ExtractPaymentRequestExport()");
            try
            {
                IList<PaymentRequestHeader> paymentRequests = await paymentRequestServices.GetPaymentRequests();
                string fileName = BuildFileName(DateTime.UtcNow.ToString(Utils.StringFirstNotNull(appSettings.FileSettings["FileDateStringFormat"], DEFAULT_DATE_FORMAT)));

                await ExtractToFileAsync(paymentRequests, fileName);

                var docUpdates = new List<PaymentRequestDocumentUpdates>();

                foreach (PaymentRequestHeader header in paymentRequests)
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

                return ExitCode.Successful;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to export payment request data. Message: {e.Message}");
                return ExitCode.CompletedButWithErrors;
            }
        }

        public async Task ExtractToFileAsync(IList<PaymentRequestHeader> paymentRequests, string fileName)
        {
            if (paymentRequests is null)
            {
                throw new ArgumentNullException(nameof(paymentRequests));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("The fileName parameter cannot be empty", nameof(fileName));
            }

            char fieldSeparator = Utils.ParseCharSetDefault(appSettings.FileSettings["FieldSeparator"],  DEFAULT_FIELD_SEPARATOR);

            logger.Debug($"Creating file {fileName}");
            var builder = new StringBuilder();
            foreach (PaymentRequestHeader header in paymentRequests)
            {
                if (appSettings.PaymentRequestOptions.LoadGLCodedLines)
                {
                    foreach (PaymentRequestGLCodedLine lineItem in header.PaymentRequestGLCodedLines)
                    {
                        var lineItemString = string.Join(fieldSeparator,
                                                        header.EntityCode,
                                                        header.VendorCode,
                                                        header.DocType,
                                                        header.CaapsRecordId,
                                                        header.DocRefNumberA,
                                                        Utils.ConvertDateTimeToString(header.DocDateIssued, appSettings.ExportFileDateFieldsFormat),
                                                        lineItem.LineTaxCode,
                                                        Utils.StringToCSVCell(header.DocDescription),
                                                        header.DocAmountTaxEx,
                                                        header.DocAmountTax,
                                                        lineItem.LineNumber,
                                                        lineItem.LineCustomFieldD,
                                                        lineItem.LineCustomFieldB,
                                                        lineItem.GLCode,
                                                        lineItem.LineAmountTaxEx,
                                                        lineItem.LineAmountTax,
                                                        Utils.StringToCSVCell(lineItem.LineDescription),
                                                        Utils.StringToCSVCell(lineItem.LineDescription),
                                                        lineItem.LineCustomFieldC);

                        builder.AppendLine(lineItemString);
                    }
                }
            }

            string fullPath;
            if (appSettings.SaveToFolder)
                fullPath = Path.Combine(appSettings.OutputFileLocation, fileName);
            else
                fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

            using (var file = new StreamWriter(fullPath))
            {
                await file.WriteAsync(builder.ToString());
            }
            logger.Debug($"File has been written to {fullPath}");

            try
            {
                if (appSettings.UploadToFTP)
                    await UploadFileToFTPAsync(fullPath).ConfigureAwait(false);
            }
            finally
            {
                await DeleteFile(fullPath).ConfigureAwait(false);
            }
        }

        private async Task DeleteFile(string fileName)
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

        private async Task UploadFileToFTPAsync(string filePath)
        {
            string hostname = Uri.EscapeUriString(appSettings.FTPSettings["Hostname"]),
                   username = appSettings.FTPSettings["Username"],
                   password = appSettings.FTPSettings["Password"];
            await FTPUtils.FTPUploadAsync(hostname, username, password, filePath);
        }
    }
}