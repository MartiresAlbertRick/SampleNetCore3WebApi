namespace AD.CAAPS.ErpVendorPortalUpload.JLLGST
{
    public class AppSettings
    {
        public string DocumentUploadEndpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string JIIGS3BucketName { get; set; }
        public string JIIGAwsAccessKeyId { get; set; }
        public string JIIGAwsSecretAccessKey { get; set; }
        public string ADAwsAccessKeyId { get; set; }
        public string ADAwsSecretAccessKey { get; set; }
        public string JIIGS3BucketRegion { get; set; }
        public bool GetCurrentDateActions { get; set; }
        public string DateFormat { get; set; }
        public string TempDownloadPath { get; set; }
    }
}