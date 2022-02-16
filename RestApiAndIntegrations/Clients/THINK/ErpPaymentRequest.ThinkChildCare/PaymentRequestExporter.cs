using System;
using System.Collections.Generic;
using System.Text;
using AD.CAAPS.Services;
using AD.CAAPS.Entities;
using NLog;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using AD.CAAPS.Common;
using AD.CAAPS.ErpPaymentRequest.Common;
using System.Security.Authentication;
using FluentFTP;

namespace AD.CAAPS.ErpPaymentRequest.ThinkChildCare
{
    class PaymentRequestExporter : BasePaymentRequestExporter<AppSettings>
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly FtpEncryptionMode encryptionMode;
        private readonly SslProtocols sslProtocols;

        protected override Logger Logger => logger;

        private void ValidateFtpSettings(string settingsPrefix, FTPSettings ftpSettings, out FtpEncryptionMode encryptionMode, out SslProtocols sslProtocols)
        {
            if (String.IsNullOrEmpty(ftpSettings.Host)) throw new ConfigurationException($"FTP host name cannot be empty {settingsPrefix}:{nameof(ftpSettings.Host)}");
            if (String.IsNullOrEmpty(ftpSettings.UserName)) throw new ConfigurationException($"FTP user name cannot be empty {settingsPrefix}:{nameof(ftpSettings.UserName)}");
            if (!Enum.TryParse<FtpEncryptionMode>(ftpSettings.EncryptionMode, out encryptionMode))
            {
                throw new ConfigurationException($"FTP encryption mode \"{ftpSettings.EncryptionMode}\" specified in the setting {settingsPrefix}:{nameof(ftpSettings.EncryptionMode)} is invalid. Valid values are \"{Enum.GetNames(typeof(FtpEncryptionMode)).Join(", ")}\".");
            }
            if (!Enum.TryParse<SslProtocols>(ftpSettings.SslProtocols, out sslProtocols))
            {
                throw new ConfigurationException($"SSL protocols \"{AppSettings.FtpSettings.SslProtocols}\" specified in the setting {settingsPrefix}:{nameof(ftpSettings.SslProtocols)} are invalid. Valid value is a comma separated list of the following values \"{Enum.GetNames(typeof(SslProtocols)).Join(", ")}\".");
            }
            if ((ftpSettings.Port < 1) || (ftpSettings.Port > UInt16.MaxValue))
            {
                throw new ConfigurationException($"Invalid FTP port {ftpSettings.Port}. Valid range 1 ... {UInt16.MaxValue}.");
            }
        }

        public PaymentRequestExporter(AppSettings appSettings, DBConfiguration dbConfiguration) : base (appSettings, dbConfiguration, new PaymentRequestServicesOptions() { LoadGLCodedLines = true })
        {
            #region Checking AppSettings
            if (String.IsNullOrWhiteSpace(appSettings.LocalTargetFolder)) throw new ConfigurationException($"Local target folder parameter {nameof(appSettings)}:{nameof(appSettings.LocalTargetFolder)} cannot be empty.");
            if (String.IsNullOrWhiteSpace(appSettings.DocumentStatusAfterExtract)) throw new ConfigurationException($"Document status after export {nameof(appSettings)}:{nameof(appSettings.DocumentStatusAfterExtract)} cannot be empty.");
            if (appSettings.UploadToFTP)
            {
                string ftpSettingsPrefix = $"{nameof(appSettings)}:{ nameof(appSettings.FtpSettings)}";
                FTPSettings ftpSettings = appSettings.FtpSettings ?? throw new ConfigurationException($"FTP settings {ftpSettingsPrefix} configuration is missing.");
                ValidateFtpSettings(ftpSettingsPrefix, ftpSettings, out encryptionMode, out sslProtocols);
            }
            if (String.IsNullOrWhiteSpace(appSettings.LocalTargetFolder)) throw new ConfigurationException($"Local target folder parameter {nameof(appSettings)}:{nameof(appSettings.LocalTargetFolder)} cannot be empty.");
            #endregion
        }

        private static CsvWriter CreateCsvWriter(string fileName, string dateFormat)
        {
            var stream = new FileStream(fileName, FileMode.CreateNew);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            CsvHelper.Configuration.CsvConfiguration defaultCsvConfiguration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            defaultCsvConfiguration.RegisterClassMap(new CsvExportMap(dateFormat));
            return new CsvWriter(writer, defaultCsvConfiguration);
        }

        private async Task UpdateDocumentStatuses(PaymentRequestServices paymentRequestServices, IList<PaymentRequestHeader> paymentRequestHeaders, string userComments)
        {
            var documentUpdates = new List<PaymentRequestDocumentUpdates>(paymentRequestHeaders.Count);
            foreach (PaymentRequestHeader header in paymentRequestHeaders)
            {
                var update = new PaymentRequestDocumentUpdates() { ExportDate = DateTime.Now, ID = header.ID, IsImportConfirmationExpected = AppSettings.IsImportConfirmationExpected, ErpImportDate = null, UserComments = userComments, Status = AppSettings.DocumentStatusAfterExtract };
                documentUpdates.Add(update);
                logger.Trace(() => $"PaymentRequestDocumentUpdates: ID={update.ID}. ExportDate={update.ExportDate}. IsImportConfirmationExpected={update.IsImportConfirmationExpected}. Status=\"{update.Status}\". UserComments=\"{update.UserComments}\".");
            }
            await paymentRequestServices.BulkUpdateDocumentStatus(documentUpdates).ConfigureAwait(false);
            logger.Debug($"Document statuses updated. Count: {documentUpdates.Count}.");
        }

        public override async Task<ExitCode> Export()
        {
            // TODO: Make PaymentRequestServices IDisposable
            IList<PaymentRequestHeader> paymentRequestHeaders = await PaymentRequestServices.GetPaymentRequests();
            if (paymentRequestHeaders.Count > 0)
            {
                var exportRowGenerator = new ExportRowGenerator(PaymentRequestServices);
                IList<ExportRow> exportRows = await exportRowGenerator.CreateExportRows(paymentRequestHeaders);
                string bareFileNameNoExtension = $"export_{DateTime.Now:yyyyMMdd-hhmmss}";
                string bareFileName = bareFileNameNoExtension + ".csv";
                string localFileName = Path.Combine(AppSettings.LocalTargetFolder, bareFileName);
                logger.Debug($"Exporting the payment requests to the local file \"{localFileName}\".");
                using (CsvWriter writer = CreateCsvWriter(localFileName, AppSettings.DateFormat))
                {
                    var exporter = new CsvExporter(writer, exportRows);
                    await exporter.ExportAsync();
                }
                logger.Info($"Payment requests have been exported to the local file \"{localFileName}\". Document count {paymentRequestHeaders.Count}. Export row count: {exportRows.Count}");
                if (AppSettings.UploadToFTP)
                {
                    await FTPUtils.FTPUploadAsync(AppSettings.FtpSettings.Host, AppSettings.FtpSettings.Port, encryptionMode, sslProtocols, AppSettings.FtpSettings.UserName, AppSettings.FtpSettings.Password, localFileName);
                    logger.Debug($"Payment request file \"{localFileName}\" has been uploaded to {AppSettings.FtpSettings.Host}.");
                }
                string userComments = AppSettings.UploadToFTP ? $"Payment request uploaded to {AppSettings.FtpSettings.Host}/{bareFileName}." : $"Payment request exported to {bareFileName}.";
                await UpdateDocumentStatuses(PaymentRequestServices, paymentRequestHeaders, userComments);
                if (AppSettings.UploadToFTP)
                {
                    string triggerFileName = Path.Combine(Path.GetTempPath(), bareFileNameNoExtension + ".trg");
                    File.WriteAllText(triggerFileName, bareFileName);
                    await FTPUtils.FTPUploadAsync(AppSettings.FtpSettings.Host, AppSettings.FtpSettings.Port, encryptionMode, sslProtocols, AppSettings.FtpSettings.UserName, AppSettings.FtpSettings.Password, triggerFileName);
                    logger.Debug($"Trigger file \"{triggerFileName}\" has been uploaded to {AppSettings.FtpSettings.Host}.");
                }
                logger.Info($"Payment requests have been uploaded to {AppSettings.FtpSettings.Host}.");
            }
            else
            {
                logger.Info("No documents ready for export were found. Nothing to do.");
            }
            return ExitCode.Successful;
        }
    }
}
