namespace AD.CAAPS.Entities
{ 
    public class ApDocumentResponse
    {
        public int Id { get; set; }
        public string CaapsId { get; set; }
        public bool Success { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string ServerFilePath { get; set; }
        public string S3BucketPath { get; set; }
    }
}