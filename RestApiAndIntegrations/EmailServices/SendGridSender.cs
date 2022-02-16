using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using Newtonsoft.Json;
using System.IO;

namespace AD.CAAPS.EmailServices
{

    public class SendGridSender : IDisposable
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public SendGridOptions Options { get; }
        protected StringBuilder ContentBuilder { get; private set; }
        public SendGridMessage Message { get; private set; }
        public SendGridClient Client { get; private set; }

        public void AppendLine(string text = null)
        {
            ContentBuilder.AppendLine(text);
        }
        public void AddAttachment(MemoryStream AttachmentStream, string FileName, string ContentType)
        {
            if (AttachmentStream is null)
            {
                throw new ArgumentNullException(nameof(AttachmentStream), "Attachment MemoryStream must be specified");
            }

            if (string.IsNullOrEmpty(FileName))
            {
                throw new ArgumentException("FileName must be specified", nameof(FileName));
            }

            if (string.IsNullOrEmpty(ContentType))
            {
                throw new ArgumentException("Attachment Content Type must be specified", nameof(ContentType));
            }

            Message.AddAttachment(new SendGrid.Helpers.Mail.Attachment()
            {
                Content = System.Convert.ToBase64String(AttachmentStream.ToArray()),
                Filename = FileName,
                Type = ContentType
            });
        }

        public void AddAttachment(string AttachmentContents, string FileName, string ContentType)
        {
            if (string.IsNullOrWhiteSpace(AttachmentContents)) return;
            if (string.IsNullOrEmpty(FileName))
            {
                throw new ArgumentException("FileName must be specified", nameof(FileName));
            }

            if (string.IsNullOrEmpty(ContentType))
            {
                throw new ArgumentException("Attachment Content Type must be specified", nameof(ContentType));
            }
            Message.AddAttachment(new SendGrid.Helpers.Mail.Attachment()
            {
                Content = System.Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(AttachmentContents.ToArray())),
                Filename = FileName,
                Type = ContentType
            });
        }

        public SendGridSender(SendGridOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Message = new SendGridMessage();
            this.ContentBuilder = new StringBuilder();
            this.Client = new SendGridClient(options.SendGridAPIKey);
        }

        public EmailAddress GetSender()
        {
            var collection = new MailAddressCollection
            {
                Options.EmailSender
            };
            if (collection.Count > 0)
            {
                MailAddress mailAddress = collection.FirstOrDefault();
                return new EmailAddress(mailAddress.Address, mailAddress.DisplayName);
            }
            else
                throw new EmailConfigurationException($"Could not parse the sender email address \"{Options.EmailSender}\".");
        }

        public List<EmailAddress> GetRecipients(bool IsErrorEmail)
        {
            string recipients = IsErrorEmail ? Options.EmailErrorRecipients : Options.EmailNotificationRecipients;
            var collection = new MailAddressCollection
            {
                // Allow using ";" to separate email addresses
                recipients.Replace(";", ",", StringComparison.InvariantCultureIgnoreCase)
            };
            var result = new List<EmailAddress>();
            foreach (MailAddress mailAddress in collection)
            {
                if (string.IsNullOrWhiteSpace(mailAddress.DisplayName))
                {
                    result.Add(new EmailAddress(mailAddress.Address, mailAddress.Address));
                }
                else
                {
                    result.Add(new EmailAddress(mailAddress.Address, mailAddress.DisplayName));
                }

            }
            if (result.Count < 1)
                throw new EmailConfigurationException($"Could not parse the recipient email address \"{recipients}\".");
            return result;
        }

        public void BuildMessageHtmlBody()
        {
            string plainText = ContentBuilder.ToString();
            Message.HtmlContent = MailUtils.MdToHtml(plainText);
            Message.PlainTextContent = plainText;
        }

        public void BuildMessageBody()
        {
            Message.PlainTextContent = ContentBuilder.ToString();
        }

        public async Task<SendEmailResponse> SendEmail(bool IsErrorEmail)
        {
            SendEmailResponse result = new SendEmailResponse() { Sent = false, ErrorDetails = null, MessageId = null };
            try
            {
                if (Message.From == null || string.IsNullOrWhiteSpace(Message.From.Email))
                {
                    Message.From = GetSender();
                }
                if (Message.Personalizations == null || Message.Personalizations.Count == 0)
                {
                    Message.AddTos(GetRecipients(IsErrorEmail));
                }
                if (string.IsNullOrWhiteSpace(Message.Subject))
                {
                    Message.Subject = $"CAAPS API email no subject configured {DateTime.UtcNow}";
                }
                if (string.IsNullOrWhiteSpace(Message.PlainTextContent))
                {
                    BuildMessageHtmlBody();
                }

                //PlainTextContent = "Please review the attachments to see the export results."
                logger.Debug("Message has been generated. Sending.");

                logger.Trace("Email subject: {Subject}", Message.Subject);
                logger.Trace("Email contents:\r\n{PlainTextContent}", Message.PlainTextContent);
                Response response = await Client.SendEmailAsync(Message).ConfigureAwait(false);

                logger.Trace("Email SendAsyncResponse.StatusCode: {StatusCode}", response.StatusCode);

                string responseBody = "";
                // Always Async/Await - Best Practices in Asynchronous Programming
                //  - Don't ...Async().Result or ...Async().Wait()
                // See: https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
                if (response.Body != null)
                {
                    responseBody = await response.Body.ReadAsStringAsync().ConfigureAwait(false);
                }

                var Headers = response.DeserializeResponseHeaders(response.Headers);

                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    result.ErrorDetails = string.Format(
                        @"An attempt to send an email report to {0}, {1} failed.
                        StatusCode: {2}
                        Headers: {3}
                        Body: {4}",
                        Options.EmailNotificationRecipients,
                        Options.EmailErrorRecipients,
                        response.StatusCode,
                        JsonConvert.SerializeObject(Headers),
                        responseBody
                    );
                }
                else
                {
                    result.Sent = true;
                    result.MessageId = Headers["X-Message-Id"];
                    logger.Info("Export report with subject {0} has been sent to {1}, {2} with SendGrid X-Message-Id {3}",
                        Message.Subject, 
                        Options.EmailNotificationRecipients,
                        Options.EmailErrorRecipients, 
                        result.MessageId);
                }
                logger.Trace("SendGrid response. Headers: \r\n{Headers}\r\nBody \r\n{Body}", Headers, responseBody);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                logger.Error(e, "Failed to send an email notification: {e.Message}");
                result.ErrorDetails = $"Failed to send an email report to {Options.EmailNotificationRecipients}, {Options.EmailErrorRecipients}. " + e.ToString();
            }

            return result;
        }

        public void AddProcessEnvironmentDetails()
        {
            ContentBuilder.AppendLine();
            ContentBuilder.AppendLine("### Process details");
            ContentBuilder.AppendLine();
            ContentBuilder.AppendLine($"* CLI: `{Environment.CommandLine}`");
            ContentBuilder.AppendLine($"* Machine: `{Environment.MachineName}`");
            ContentBuilder.AppendLine($"* O/S: `{Environment.OSVersion}`");
            ContentBuilder.AppendLine($"* 64bit O/S: `{Environment.Is64BitOperatingSystem}`, 64bit process: `{Environment.Is64BitProcess}`");
            ContentBuilder.AppendLine($"* Stack: `{Environment.Version}`");
            ContentBuilder.AppendLine($"* Local server time: `{MailUtils.CurrentLocalTimeAsJson()}`");
            ContentBuilder.AppendLine($"* UTC server time: `{MailUtils.CurrentUtcTimeAsJson()}`");
            ContentBuilder.AppendLine();
            var coreAssemblyInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(object).Assembly.Location);
            ContentBuilder.AppendLine($"* Framework product version: `{coreAssemblyInfo.ProductVersion}`");
            ContentBuilder.AppendLine($"* File version: `{coreAssemblyInfo.FileVersion}`");
            ContentBuilder.AppendLine($"* Location: `{typeof(object).Assembly.Location}`");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.ContentBuilder = null;
                    this.Message = null;
                    this.Client = null;
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

}
