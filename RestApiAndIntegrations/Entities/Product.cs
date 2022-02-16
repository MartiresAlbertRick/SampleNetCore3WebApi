using System;

namespace AD.CAAPS.Entities
{
    public class Product
    {
        public int ID { get; set; }
        public string VendorBusinessNumber { get; set; }
        public string VendorName { get; set; }
        public string VendorProductCode { get; set; }
        public string VendorProductDescription { get; set; }
        public string VendorUOM { get; set; }
        public string PoProductCode { get; set; }
        public string PoProductDescription { get; set; }
        public string PoUOM { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}