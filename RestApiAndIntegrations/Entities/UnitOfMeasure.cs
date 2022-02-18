using System;

namespace AD.CAAPS.Entities
{
    public partial class UnitOfMeasure
    {
        public int ID { get; set; }
        public string UOMCode { get; set; }
        public string UOMDescription { get; set; }
        public string UOMAlternatives { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public string UpdatedByUser { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}