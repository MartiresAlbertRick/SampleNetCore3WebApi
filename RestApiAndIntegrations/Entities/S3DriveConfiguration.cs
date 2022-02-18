namespace AD.CAAPS.Entities
{
    public class S3DriveConfiguration
    {
        public int ID { get; set; }
        public string PhysicalDriveToken { get; set; }
        public int DefaultTimeoutInMinutes { get; set; }
        public int ResignBufferThresholdInMinutes { get; set; }
        public string S3Bucket { get; set; }
        public string S3Profile { get; set; }
        public string S3Region { get; set; }
        public string AWSCLIExePath { get; set; }
    }
}