using System;

namespace AD.CAAPS.Entities
{
    public partial class ValidAdditionalCharges
    {
        public int ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string ValidAdditionalCharge { get; set; }
        public string DefaultGLCode { get; set; }
        public string ValidAlternatives { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string UpdatedByUser { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}