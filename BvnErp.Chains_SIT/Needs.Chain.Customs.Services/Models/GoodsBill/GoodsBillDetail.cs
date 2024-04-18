using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class GoodsBillDetail
    {
        public string ID { get; set; }
        public int GNo { get; set; }
        public string CodeTS { get; set; }
        public string GName { get; set; }
        public string GoodsBrand { get; set; }

        public string GoodsModel { get; set; }
        public string GModel { get; set; }
        public string GUnit { get; set; }
        public decimal GQty { get; set; }
        public decimal DeclPrice { get; set; }

        public decimal DeclTotal { get; set; }
        public decimal? TaxedPrice { get; set; }
        public string TradeCurr { get; set; }
        public string CaseNo { get; set; }
        public decimal? NetWt { get; set; }

        public decimal? GrossWt { get; set; }
        public string OriginCountry { get; set; }
        public string ContrNo { get; set; }
        public string EntryId { get; set; }
        public DateTime? DDate { get; set; }

        public decimal tariffRate { get; set; }
        public string OwnerName { get; set; }
        public string CiqCode { get; set; }
        public string TaxName { get; set; }
        public string TaxCode { get; set; }

        public string OrderID { get; set; }
        public int OrderType { get; set; }
        public string IcgooOrder { get; set; }
        public string OperatorName { get; set; }
        public DateTime? InStoreDate { get; set; }
        public string tariffTN { get; set; }
        public decimal tariffAmount { get; set; }

        public DateTime? DeductionMonth { get; set; }
        public string valueAddedTN { get; set; }
        public decimal? valueAddedAmount { get; set; }
        public string ConsumptionTN { get; set; }
        public decimal ConsumptionAmount { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public decimal? InvoiceTaxAmount
        {
            get
            {
                decimal amount = 0;
                if (this.InvoiceType == 0&&this.InvoiceAmount!=null)
                {
                    amount = Math.Round(this.InvoiceAmount.Value/(1+this.InvoiceTaxRate)*this.InvoiceTaxRate,2);
                }               
                return amount;
            }
        }
        public string InvoiceNo { get; set; }
        public int InvoiceType { get; set; }
        public string InvoiceNoticeID { get; set; }
        public decimal InvoiceTaxRate { get; set; }

        public string WaybillCode { get; set; }
        public string VoyNo { get; set; }

        public string AdminID { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public string ClientCode { get; set; }
        public string DeclarationID { get; set; }
        public string OrderItemID { get; set; }
        public string GunitName { get; set; }
        public string OperatorID { get; set; }
        public string LotNumber { get; set; }
    }
}
