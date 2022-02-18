namespace AD.CAAPS.EmailServices
{
    public class SendGridOptions
    {
        public string EmailSender { get; set; }
        public string EmailNotificationRecipients { get; set; }
        public string EmailErrorRecipients { get; set; }
        public string SendGridAPIKey { get; set; }
    }

}
