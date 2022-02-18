using System;
using System.Collections.Generic;
using System.Linq;

namespace AD.CAAPS.Entities
{
    public partial class PurchaseOrder
    {
        public int ID { get; set; }
        public string EntityCode { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoRaisedByUsername { get; set; }
        public string PoRequisitionerUserName { get; set; }
        public string PoNumber { get; set; }        
        public decimal? PoOriginalAmountTaxEx { get; set; }
        public decimal? PoOpenAmountTaxEx { get; set; }
        public string PoType { get; set; }
        public string PoStatusCode { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? LineExpectedDate { get; set; }
        public string LineNumber { get; set; }
        public string LineProductCode { get; set; }
        public decimal? LineOriginalQuantity { get; set; }
        public decimal? LineOpenQuantity { get; set; }
        public decimal? LineOriginalAmountTaxEx { get; set; }
        public decimal? LineOpenAmountTaxEx { get; set; }
        public string LineUOM { get; set; }
        public decimal? LineUnitPrice { get; set; }
        public string LineTaxCode { get; set; }
        public string LineGLCode { get; set; }
        public string LineDescription { get; set; }
        public string VendorCode { get; set; }
        public string BranchCode { get; set; }
        public string DivisionCode { get; set; }
        public string SiteCode { get; set; }
        public string BusinessUnitCode { get; set; }
        public string Custom01 { get; set; }
        public string Custom02 { get; set; }
        public string Custom03 { get; set; }
        public string Custom04 { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public void SetAllocatedGoodsReceipts(List<GoodsReceipt> goodsReceipts)
        {
            GoodsReceipts.Clear();
            GoodsReceipts.AddRange(goodsReceipts);
        }

        public List<GoodsReceipt> GoodsReceipts { get; } = new List<GoodsReceipt>();
    }
}