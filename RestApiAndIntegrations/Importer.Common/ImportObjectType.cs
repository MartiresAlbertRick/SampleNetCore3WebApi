using System.ComponentModel;

namespace AD.CAAPS.Importer.Common
{
    public enum ImportObjectType
    {
        [Description("Unknown")]
        Unknown = 0,

        [Description("Vendor")]
        Vendor = 1,

        [Description("Goods Receipt")]
        GoodsReceipt = 2,

        [Description("Purchase Order")]
        PurchaseOrder = 3,

        [Description("Import Confirmation")]
        ImportConfirmation = 4,

        [Description("Payment")]
        Payment = 5,

        [Description("Entity")]
        Entity = 6,

        [Description("Valid Additional Charges")]
        ValidAdditionalCharges = 7,

        [Description("GL Code Details")]
        GLCodeDetails = 8,

        [Description("Closed Purchase Order")]
        ClosedPurchaseOrder = 9,

        [Description("Non Po Vendor")]
        NonPoVendor = 10,

        [Description("Payment Terms")]
        PaymentTerms = 11,

        [Description("Product")]
        Product = 12,

        [Description("Routing Codes")]
        RoutingCodes = 13,

        [Description("Tax Code Details")]
        TaxCodeDetails = 14,

        [Description("Unit Of Measure")]
        UnitOfMeasure = 15
    }
}