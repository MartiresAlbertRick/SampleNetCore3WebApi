namespace AD.CAAPS.Entities
{
    public partial class ImportConfirmation
    {
        public int ID { get; set; }
        public string CaapsUniqueId { get; set; }
        public string ImportStatus { get; set; }
        public string ImportMessage { get; set; }
        public string ClientTransactionId { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public System.DateTime? LastModifiedDateTime { get; set; }
    }
}