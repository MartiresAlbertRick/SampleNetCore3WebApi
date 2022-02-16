namespace AD.CAAPS.Importer.Urbanise
{
    public class Supplier
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string AccountBsb { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BpayBillerCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }
        public string SupplierABN { get; set; }
        public bool RegisteredForGST { get; set; }
        public bool UseEFT { get; set; }
        public string GstRegistrationNumber { get; set; }
    }
}