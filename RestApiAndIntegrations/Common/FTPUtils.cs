using FluentFTP;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Net.Security;

namespace AD.CAAPS.Common
{
    public static class FTPUtils
    {
        public const FtpEncryptionMode DefaultEncryptionMode = FtpEncryptionMode.Implicit;
        public const SslProtocols DefaultSslProtocols = SslProtocols.None;
        public const int DefaultPort = 990;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task FTPUploadAsync(string hostName, string userName, string password, string fileName)
        {
            await FTPUploadAsync(hostName, DefaultPort, DefaultEncryptionMode, DefaultSslProtocols, userName, password, fileName);
        }

        public static async Task FTPUploadAsync(string hostName, int port, FtpEncryptionMode encryptionMode, SslProtocols sslProtocols, string userName, string password, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Local FileName not specified for upload.", nameof(fileName));
            }
            if (File.Exists(fileName))
            {
                logger.Debug(() => $"Attempting to upload file from \"{fileName}\" to FTP host {hostName}:{port}. User name \"{userName}\". Encryption mode: {encryptionMode}. SSL protocols: {sslProtocols}. To see the password enable Trace level logging.");
                logger.Trace(() => $"Password \"{password}\".");
                using var client = new FtpClient(hostName, port, userName, password)
                {
                    EncryptionMode = encryptionMode,
                    SslProtocols = sslProtocols
                };
                client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);
                logger.Debug($"Attempting to connect to FTP server {hostName}:{port}");
                await client.ConnectAsync().ConfigureAwait(false);
                if (client.IsConnected)
                {
                    logger.Debug($"Succesfully connected to FTP server {hostName}:{port}");
                    var fileInfo = new FileInfo(fileName);
                    var ftpStatus = await client.UploadFileAsync(fileName, fileInfo.Name, FtpRemoteExists.Overwrite).ConfigureAwait(false);
                    if (ftpStatus == FtpStatus.Success)
                    {
                        logger.Debug($"File {fileInfo.Name} successfully uploaded to FTP {hostName}");
                    }
                    else
                    {
                        throw new FtpException($"Upload of {fileInfo.Name} to {hostName} failed with FtpStatus {ftpStatus}.");
                    }
                }
                else
                {
                    throw new FtpException($"Unable to connect to FTP server {hostName}");
                }
            }
            else
            {
                throw new FtpException($"Cannot upload specified file - it does not exist: {fileName}");
            }
        }

        private static void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            logger.Trace(() => $"FTP SSL Connected. SSL Cert - Subject: {e.Certificate.Subject}, Issuer: {e.Certificate.Issuer}");
            if (e.PolicyErrors != SslPolicyErrors.None)
            {
                logger.Trace(() => $"FTP SSL Policy errors - forcing accept and ignoring policy errors: {e.PolicyErrors}");
            }
            e.Accept = true;
        }
    }
}
