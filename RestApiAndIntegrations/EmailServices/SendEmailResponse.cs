namespace AD.CAAPS.EmailServices
{
    public class SendEmailResponse
    {
        public bool Sent { get; set; }
        public string ErrorDetails { get; set; }
        public string MessageId { get; set; }
    }

}
