namespace AD.CAAPS.Entities
{
    public partial class LineItem
    {
        public int ID { get; set; }
        public int? RecordId { get; set; }
        public string LineHeaderUID { get; set; }
        public int? LineNumber { get; set; }
        public string LinePoNumber { get; set; }
        public string LineOriginalPoNumber { get; set; }
        public string LineProductCode { get; set; }
        public string LineOriginalProductCode { get; set; }
        public string LineValidAdditionalCharge { get; set; }
        public decimal? LineQuantity { get; set; }
        public string LineUOM { get; set; }
        public decimal? LineUnitAmountTaxEx { get; set; }
        public decimal? LineAmountTax { get; set; }
        public decimal? LineAmountTaxEx { get; set; }
        public decimal? LineAmountTotal { get; set; }
        public string LineDescription { get; set; }
        public string PoIssuedBy { get; set; }
        public string PoType { get; set; }
        public string LinePoLineNumber { get; set; }
        public string LineTaxCode { get; set; }
        public decimal? OriginalUnitAmount { get; set; }
        public decimal? LineOriginalAmountTotal { get; set; }
    }
}