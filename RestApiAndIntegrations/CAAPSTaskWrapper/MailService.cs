using AD.CAAPS.EmailServices;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AD.CAAPSTaskWrapper
{
    public class MailService : IMailService
    {
        private const int MaxBodyProgramOutputCharacters = 1000;
        public MailService(ILogger logger, IConfiguration configuration)
        {
            this.Logger = logger;
            this.Configuration = configuration;
        }

        private ILogger Logger { get; }
        private IConfiguration Configuration { get; }

        public async Task<bool> SendEmailReport(string runOptionsPath, string runOptionsSwitches, DateTime startTime, int exitCode, string stdOutput, string errOutput, bool verbose = false)
        {
            bool IsError = exitCode != 0;
            try
            {
                string emailSubject = $"{(IsError ? "Error running: " : "Notification: ")} {Path.GetFileNameWithoutExtension(runOptionsPath)} @ {DateTime.UtcNow.ToShortDateString()}";
                if (verbose)
                {
                    if (IsError)
                    {
                        Logger.Error($"ExitCode: {exitCode}");
                        Logger.Error($"StdOut: {stdOutput}");
                        Logger.Error($"StdErr: {errOutput}");
                    }
                    else
                    {
                        Logger.Info($"ExitCode: {exitCode}");
                        Logger.Info($"StdOut: {stdOutput}");
                        Logger.Info($"StdErr: {errOutput}");
                    }
                }
                var options = Configuration.GetSection("AppSettings:EmailNotifications").Get<SendGridOptions>();
                if (options == null)
                {
                    Logger.Error($"Email notifications disabled - AppSettings:EmailNotifications section is not configured");
                    return !IsError;
                }

                Logger.Debug($"Email Sender: \"{options.EmailSender}\"");
                Logger.Debug($"Email Notification Recipients: \"{options.EmailNotificationRecipients}\"");
                Logger.Debug($"Email Error Recipients: \"{options.EmailErrorRecipients}\"");

                if (string.IsNullOrWhiteSpace(options.EmailSender))
                {
                    Logger.Error($"Email notifications disabled - AppSettings:EmailNotifications:EmailSender not configured");
                    return !IsError;
                }
                if (exitCode == 0 && string.IsNullOrWhiteSpace(options.EmailNotificationRecipients))
                {
                    Logger.Error($"Email notifications disabled - AppSettings:EmailNotifications:EmailNotificationRecipients not configured");
                    return !IsError;
                }
                if (exitCode != 0 && string.IsNullOrWhiteSpace(options.EmailErrorRecipients))
                {
                    Logger.Error($"Email notifications disabled - AppSettings:EmailNotifications:EmailErrorRecipients not configured");
                    return !IsError;
                }
                if (string.IsNullOrWhiteSpace(options.SendGridAPIKey))
                {
                    Logger.Error($"Email notifications disabled - AppSettings:SendGridAPIKey not configured");
                    return !IsError;
                }
                using var notification = new SendGridSender(options);
                notification.Message.Subject = emailSubject;
                notification.AppendLine($"### Program called");
                notification.AppendLine("");
                notification.AppendLine($"Path: \"{runOptionsPath}\"");
                notification.AppendLine("");
                notification.AppendLine($"Args: \"{runOptionsSwitches}\"");
                notification.AppendLine("");
                notification.AppendLine($"* Started at: `{startTime:yyyy'-'MM'-'dd HH':'mm':'ss}`");
                notification.AppendLine($"* Finished at: `{DateTime.Now:yyyy'-'MM'-'dd HH':'mm':'ss}`");
                notification.AppendLine("");
                notification.AppendLine($"ExitCode: `{exitCode}`");
                notification.AppendLine("");
                if (!string.IsNullOrWhiteSpace(errOutput))
                {
                    notification.AppendLine("### Error details");
                    notification.AppendLine("");
                    notification.AppendLine("```");
                    notification.AppendLine(errOutput);
                    notification.AppendLine("```");
                    notification.AppendLine("");
                }
                if (!string.IsNullOrWhiteSpace(stdOutput))
                {
                    notification.AppendLine("### Program output");
                    notification.AppendLine("");
                    notification.AppendLine("```");
                    if (stdOutput.Length > MaxBodyProgramOutputCharacters)
                    {
                        notification.AppendLine(stdOutput.Substring(0, MaxBodyProgramOutputCharacters));
                        notification.AppendLine("...truncated... see attachments...");
                    }
                    else
                    {
                        notification.AppendLine(stdOutput);
                    }
                    notification.AppendLine("```");
                    notification.AppendLine("");
                    notification.AddAttachment(stdOutput, "ProgramOutput.txt", "text/plain");
                }
                notification.AddProcessEnvironmentDetails();
                notification.BuildMessageHtmlBody();
                var response = await notification.SendEmail(IsError);
                if (!response.Sent)
                {
                    Logger.Error($"Email notification failed: {response.ErrorDetails}");
                    return !IsError;
                }
                else
                {
                    return true;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Logger.Error(ex, $"Email notification failed: {ex.Message}");
                return false;
            }
        }
    }
}
