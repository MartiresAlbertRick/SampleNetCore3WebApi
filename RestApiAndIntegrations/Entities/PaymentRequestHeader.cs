using System.Collections.Generic;

namespace AD.CAAPS.Entities
{
    public partial class PaymentRequestHeader : ApDocument
    {
        public void SetPaymentRequestGLCodedLines(IList<PaymentRequestGLCodedLine> paymentRequestGLCodedLines)
        {
            PaymentRequestGLCodedLines.Clear();
            PaymentRequestGLCodedLines.AddRange(paymentRequestGLCodedLines);
        }

        public void SetPaymentRequestPOMatchedLines(IList<PaymentRequestPOMatchedLine> paymentRequestPOMatchedLines)
        {
            PaymentRequestPOMatchedLines.Clear();
            PaymentRequestPOMatchedLines.AddRange(paymentRequestPOMatchedLines);
        }

        public void SetSundryPOGoodsAllocations(IList<SundryPOGoodsAllocation> sundryPOGoodsAllocations)
        {
            SundryPOGoodsAllocations.Clear();
            SundryPOGoodsAllocations.AddRange(sundryPOGoodsAllocations);
        }

        public void SetPurchaseOrderGoodsReceiptAllocation(IList<PurchaseOrder> purchaseOrders)
        {
            PurchaseOrderGoodsReceiptAllocation.Clear();
            PurchaseOrderGoodsReceiptAllocation.AddRange(purchaseOrders);
        }

        public List<PaymentRequestGLCodedLine> PaymentRequestGLCodedLines { get; } = new List<PaymentRequestGLCodedLine>();
        public List<PaymentRequestPOMatchedLine> PaymentRequestPOMatchedLines { get; } = new List<PaymentRequestPOMatchedLine>();
        public List<SundryPOGoodsAllocation> SundryPOGoodsAllocations { get; } = new List<SundryPOGoodsAllocation>();
        public List<PurchaseOrder> PurchaseOrderGoodsReceiptAllocation { get; } = new List<PurchaseOrder>();
    }
}