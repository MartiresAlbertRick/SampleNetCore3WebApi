using AD.CAAPS.Common;
using Newtonsoft.Json;
using NLog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AD.CAAPS.ErpPaymentRequest.Common;

namespace AD.CAAPS.ErpVendorPortalUpload.JLLGST
{
    public class VendorPortalUpload
    {
        private readonly string connectionString;
        private readonly AppSettings appSettings;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly DateTime? lastActionDate;
        public string PhysicalDriveToken { get; set; }
        public string S3Bucket { get; set; }

        public VendorPortalUpload(string connectionString, AppSettings appSettings, DateTime? lastActionDate)
        {
            this.connectionString = connectionString;
            this.appSettings = appSettings;
            this.lastActionDate = lastActionDate;
        }

        public async Task<ExitCode> Export()
        {
            logger.Debug("Starting to process exports");
            ExitCode exitCode;
            
            string downloadPath = FileUtils.CheckPathNotExistThenCreate(appSettings.TempDownloadPath);
            await LoadADS3BucketDetails().ConfigureAwait(false);
            
            List<ExportData> collection = await ExtractFromDatabase(appSettings.GetCurrentDateActions).ConfigureAwait(false);

            if (collection.Count < 1)
            {
                logger.Debug("No records to process");
                exitCode = ExitCode.CompletedButWithUnprocessed;
            }
            else
            {
                int successUpload = 0, failedUpload = 0;
                var finalData = new List<ExportData>();
                using (Amazon.S3.IAmazonS3 caapsS3client = AmazonS3Utils.CreateClient(appSettings.ADAwsAccessKeyId, appSettings.ADAwsSecretAccessKey))
                using (Amazon.S3.IAmazonS3 erpS3client = AmazonS3Utils.CreateClient(appSettings.JIIGAwsAccessKeyId, appSettings.JIIGAwsSecretAccessKey, appSettings.JIIGS3BucketRegion))
                {
                    foreach (ExportData data in collection)
                    {
                        foreach (File file in data.Files)
                        { 
                            string fileName = "";
                            try
                            {
                                string s3BucketPath = AmazonS3Utils.BuildS3BucketPath(file.SourceFilePath, PhysicalDriveToken, S3Bucket);
                                fileName = Path.Combine(downloadPath, file.FileName);

                                await AmazonS3Utils.DownloadFileAsync(caapsS3client, fileName, s3BucketPath, file.FileName).ConfigureAwait(false);
                                file.DestinationFilePath = string.Join("/", appSettings.JIIGS3BucketName, file.FileName);
                                await AmazonS3Utils.UploadFileAsync(erpS3client, fileName, appSettings.JIIGS3BucketName, file.FileName).ConfigureAwait(false);
                                logger.Debug($"invoice with file name {file.FileName} successfully uploaded");
                                successUpload++;
                                finalData.Add(data);
                            }
                            catch (Exception e)
                            {
                                logger.Error(e, $"An exception caught while uploading pdf document {file.FileName}: {e.Message}");
                                failedUpload++;
                            }
                            finally
                            {
                                // DeleteFile function already includes checking of file existence
                                // Delete from temp folder
                                await FileUtils.DeleteFile(fileName).ConfigureAwait(false);
                            }

                        }
                    }
                }
                logger.Debug($"Processed {collection.Count}");
                logger.Debug($"Success upload {successUpload}");
                logger.Debug($"Failed upload {failedUpload}");

                var exportList = new ExportList();
                exportList.SetData(finalData);

                var request = RestSharpUtils.CreateRequestForPostJson();
                logger.Debug($"Attempting to send JSON to {appSettings.DocumentUploadEndpoint}. JSON: {JsonConvert.SerializeObject(exportList)}");
                request.AddParameter("application/json", JsonConvert.SerializeObject(exportList), RestSharp.ParameterType.RequestBody);
                var restSharptUtils = new RestSharpUtils(new Uri(appSettings.DocumentUploadEndpoint), request);
                RestSharp.IRestResponse response = await restSharptUtils.SendRequestAsync().ConfigureAwait(false);

                if (!RestSharpUtils.IsResposeSuccess(response))
                {
                    exitCode = ExitCode.CompletedButWithErrors;
                    logger.Error($"Request failed on sending data to {restSharptUtils.RequestUrl}. Response: {RestSharpUtils.GetRestResponseErrorDetails(response)}");
                }
                else
                {

                    if (failedUpload > 0) {
                        exitCode = ExitCode.CompletedButWithErrors;
                    }
                    else
                    {
                        exitCode = ExitCode.Successful;
                    }
                    logger.Debug($"Request success. Response \r\n{response.Content}\r\n");
                }
            }
            logger.Debug($"Program has ended. Exited at code {exitCode}");
            return exitCode;
        }

        public async Task<List<ExportData>> ExtractFromDatabase(bool getCurrentDateActions)
        {
            var result = new List<ExportData>();
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using SqlCommand command = conn.CreateCommand();
                command.CommandType = CommandType.Text;
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                command.CommandText = GetSqlQuery(getCurrentDateActions);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

                if (lastActionDate != null)
                    command.Parameters.AddWithValue("@currentDate", Utils.ConvertDateTimeToString(lastActionDate, appSettings.DateFormat));
                else if (getCurrentDateActions)
                    command.Parameters.AddWithValue("@currentDate", DateTime.UtcNow.ToString(appSettings.DateFormat));

                logger.Trace($"SQL: {command.CommandText}");
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                if (reader.HasRows)
                {
                    int counter = 0;
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        counter++;
                        var data = new ExportData
                        {
                            ID = Utils.SafeGetInt(reader, 0),
                            VendorCode = Utils.SafeGetString(reader, 1),
                            VendorName = Utils.SafeGetString(reader, 2),
                            VendorEmailID = Utils.SafeGetString(reader, 3),
                            ClientName = Utils.SafeGetString(reader, 4),
                            CaapsID = Utils.SafeGetString(reader, 5),
                            DocRefNumber = Utils.SafeGetString(reader, 6),
                            PoNumber = Utils.SafeGetString(reader, 7),
                            PoProcessingStatus = Utils.SafeGetString(reader, 8),
                            ProcessingStatusDate = Utils.SafeGetString(reader, 9),
                            ProcessingRemarks = Utils.SafeGetString(reader, 10),
                            BusinessUnit = Utils.SafeGetString(reader, 11),
                            DateIssued = Utils.SafeGetString(reader, 12),
                            ReceivedDate = Utils.SafeGetString(reader, 13),
                            SgstUtgstAmount = Utils.SafeGetDouble(reader, 14),
                            CgstAmount = Utils.SafeGetDouble(reader, 15),
                            IgstAmount = Utils.SafeGetDouble(reader, 16),
                            AmountExcludingTax = Utils.SafeGetDouble(reader, 17),
                            TaxAmount = Utils.SafeGetDouble(reader, 18),
                            TotalAmount = Utils.SafeGetDouble(reader, 19),
                            VendorState = Utils.SafeGetString(reader, 20),
                            BusinessUnitState = Utils.SafeGetString(reader, 21),
                            DocumentType = Utils.SafeGetString(reader, 22),
                            RecordOwner = Utils.SafeGetString(reader, 23),
                            Cess = Utils.SafeGetDouble(reader, 24),
                            TaxCode = Utils.SafeGetString(reader, 25),
                            DueDate = Utils.SafeGetString(reader, 26),
                            PaidDate = Utils.SafeGetString(reader, 27),
                            FmApprovalDate = Utils.SafeGetString(reader, 28),
                            FinanceManagerApprovalDate = Utils.SafeGetString(reader, 29),
                            FscApprovalDate = Utils.SafeGetString(reader, 30),
                            GrApprovalDate = Utils.SafeGetString(reader, 31),
                            ClientApprovalDate = Utils.SafeGetString(reader, 32),
                            CaapsExportToJdeDate = Utils.SafeGetString(reader, 33),
                            JdePaymentNotificationDate = Utils.SafeGetString(reader, 34),
                            LastActionDate = Utils.SafeGetString(reader, 35),
                            ClientPaymentDueDate = Utils.SafeGetString(reader, 36),
                            ExpectedVendorPaymentDate = Utils.SafeGetString(reader, 37),
                            PaymentNumber = Utils.SafeGetString(reader, 38),
                            PaymentAmount = Utils.SafeGetDouble(reader, 39),
                            WithholdingTaxes = Utils.SafeGetString(reader, 40),
                            EmailCirculatedOnDateTimeInIst = Utils.SafeGetString(reader, 41),
                            ClientInvoiceNumber = Utils.SafeGetString(reader, 42)
                        };

                        data.SetFiles(new List<File> {
                                    new File {
                                        SourceFilePath = Utils.SafeGetString(reader, 43),
                                        FileName = Utils.SafeGetString(reader, 44)
                                    }
                                });
                        result.Add(data);
                    }
                    logger.Info($"Fetched records: {counter}");
                }
            }
            return result;
        }

        public string GetSqlQuery(bool getCurrentDateActions)
        {
            string condition = "WHERE D.VENDOR_CODE IS NOT NULL ";
            if (getCurrentDateActions || lastActionDate != null)
                condition += "AND CONVERT(NVARCHAR, D.PROCESS_LAST_ACTION_DATE, 105) = @currentDate ";

            return @"SELECT D.ID, 
                          D.VENDOR_CODE,
                          D.VENDOR_NAME,
                          D.VENDOR_EMAIL_ADDRESS,
                          D.CLIENT_NAME,
                          D.DV_RECORD_ID,
                          D.DOC_REF_NUMBER_A,
                          D.PO_NUMBER,
                          D.PROCESS_STATUS_CURRENT,
                          CONVERT(NVARCHAR, D.PROCESS_LAST_ACTION_DATE, 105) PROCESS_LAST_ACTION_DATE,
                          CASE WHEN D.PROCESS_STATUS_CURRENT = 'REJECTED' 
                            THEN D.REJECT_REASON
                            ELSE D.COMMENTS
                          END [PROCESSING_REMARKS],
                          D.ENTITY_SITE_CODE [BUSINESS_UNIT],
                          CONVERT(NVARCHAR, D.DOC_DATE_ISSUED, 105) DOC_DATE_ISSUED,
                          CONVERT(NVARCHAR, D.AD_DOC_DATE_RECEIVED, 105) AD_DOC_DATE_RECEIVED,
                          D.DOC_AMOUNT_TAX_SGST_UTGST,
                          D.DOC_AMOUNT_TAX_CGST,
                          D.DOC_AMOUNT_TAX_IGST,
                          D.DOC_AMOUNT_TAX_EX,
                          D.DOC_AMOUNT_TAX,
                          D.DOC_AMOUNT_TOTAL,
                          D.VENDOR_STATE,
                          D.ENTITY_INVOICE_STATE [BUSINESS_UNIT_STATE],
                          D.DOC_TYPE,
                          D.REC_OWNER_CURRENT,
                          D.DOC_AMOUNT_COMPENSATION_CESS [CESS],
                          D.DOC_TAX_CODE,
                          CONVERT(NVARCHAR, D.DOC_DATE_DUE, 105) DOC_DATE_DUE,
                          CONVERT(NVARCHAR, D.DOC_DATE_PAID, 105) DOC_DATE_PAID,
                          CONVERT(NVARCHAR, C.FM_APPROVAL_DATE, 105) FM_APPROVAL_DATE,
                          CONVERT(NVARCHAR, C.FINANCE_MANAGER_APPROVAL_DATE, 105) FINANCE_MANAGER_APPROVAL_DATE,
                          CONVERT(NVARCHAR, C.FSC_APPROVAL_DATE, 105) FSC_APPROVAL_DATE,
                          CONVERT(NVARCHAR, C.GR_APPROVAL_DATE, 105) GR_APPROVAL_DATE,
                          CONVERT(NVARCHAR, C.CLIENT_APPROVAL_DATE, 105) CLIENT_APPROVAL_DATE,
                          CONVERT(NVARCHAR, C.EXPORT_DATE, 105) EXPORT_DATE,
                          CONVERT(NVARCHAR, C.PAYMENT_NOTFICATION_DATE, 105) PAYMENT_NOTFICATION_DATE,
                          CONVERT(NVARCHAR, D.PROCESS_LAST_ACTION_DATE, 105) LAST_ACTION_DATE,
                          NULL CLIENT_PAYMENT_DUE,
                          CONVERT(NVARCHAR, D.DOC_DATE_DUE, 105) [EXPECTED_VENDOR_PAYMENT_DATE],
                          P.PAYMENT_VOUCHER_NUMBER [PAYMENT_NUMBER],
                          P.PAYMENT_AMOUNT [PAYMENT_AMOUNT],
                          P.TDS_DEDUCTED [WITHHOLDING_TAXES],
                          CONVERT(NVARCHAR, D.PROCESS_LAST_ACTION_DATE, 105) [EMAIL_CIRCULATED_ON_DATETIME_IN_IST],
                          NULL [CLIENT_INVOICE_NUMBER],
                          FN.FILEPATH,
                          FN.FILENAME
                   FROM DRAWINGS D INNER JOIN CAAPS_INVOICE_LIFECYCLE_EVENTS C
                   ON D.ID = C.RECORDID
                   LEFT JOIN FILELINKS FL
                   ON D.ID = FL.RECORDID
                   LEFT JOIN FILENAMES FN
                   ON FL.FILEID = FN.ID
                   LEFT JOIN BRE_CUSTOM_PAYMENTS P
                   ON D.DV_RECORD_ID = P.CAAPS_UNIQUE_ID " + condition;
        }

        public async Task LoadADS3BucketDetails()
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using SqlCommand command = conn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT PHYSICAL_DRIVE_TOKEN, S3_BUCKET FROM S3_DRIVE_CONFIGURATION";

            using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (reader.HasRows)
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    PhysicalDriveToken = Utils.SafeGetString(reader, 0);
                    S3Bucket = Utils.SafeGetString(reader, 1);
                }
            }
        }
    }
}