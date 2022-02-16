namespace AD.CAAPS.Entities
{
    public partial class GLCodeLine
    {
        public int ID { get; set; }
        public int? RecordId { get; set; }
        public int? LineNumber { get; set; }
        public string LineAccountType { get; set; }
        public string LineAccountCodeA { get; set; }
        public string LineAccountCodeB { get; set; }
        public string LineAccountCodeC { get; set; }
        public string LineAccountCodeD { get; set; }
        public string LineAccountCodeE { get; set; }
        public string LineAccountCodeF { get; set; }
        public decimal? LineAmountTax { get; set; }
        public decimal? LineAmountTaxEx { get; set; }
        public decimal? LineAmountTotal { get; set; }
        public bool? LineApprovedYN { get; set; }
        public decimal? LineCalcPercent { get; set; }
        public string LineDescription { get; set; }
        public string LineCustomFieldA { get; set; }
        public string LineCustomFieldB { get; set; }
        public string LineCustomFieldC { get; set; }
        public string LineCustomFieldD { get; set; }
        public string LineTaxCode { get; set; }
        public string GLCode { get; set; }
        public string GLCodeDesc { get; set; }
        public int? LineVACLineNumber { get; set; }
        public string LineVACDesc { get; set; }
    }
}