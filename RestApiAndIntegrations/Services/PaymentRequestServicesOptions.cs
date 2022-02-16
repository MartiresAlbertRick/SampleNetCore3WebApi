namespace AD.CAAPS.Services
{
    public class PaymentRequestServicesOptions
    {
        public bool LoadGLCodedLines { get; set; }
        public bool LoadPOMatchedLines { get; set; }
        public bool LoadSundryPOAllocations { get; set; }
        public bool LoadPOGRAllocations { get; set; }
    }
}