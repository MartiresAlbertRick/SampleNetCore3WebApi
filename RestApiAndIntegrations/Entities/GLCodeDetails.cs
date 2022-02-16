using System;

namespace AD.CAAPS.Entities
{
    public class GLCodeDetails
    {
        public int ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string GLCode { get; set; }
        public string GLCodeDescription { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string UpdatedByUser { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}