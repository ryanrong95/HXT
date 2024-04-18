using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StorageFeeReport
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string Business { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        /// <summary>
        /// 应收金额（参考价）
        /// </summary>
        public decimal? ReceivableAmount { get; set; }
        /// <summary>
        /// 应收金额（参考价） RMB金额
        /// </summary>
        public decimal? ReceivableCNYAmount { get; set; }
        /// <summary>
        /// 应付未税金额
        /// </summary>
        public decimal PayableAmount { get; set; }
        /// <summary>
        /// 应付含税税金额
        /// </summary>
        public decimal PayableTaxedAmount
        {
            get
            {
                return Math.Round(this.PayableAmount * 1.06m,2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal Discount
        {
            get
            {
                if (this.ReceivableAmount == null)
                {
                    return 0;
                }
                else
                {
                    return Math.Round(this.ReceivableCNYAmount.Value,2,MidpointRounding.AwayFromZero) - Math.Round(this.PayableAmount,2, MidpointRounding.AwayFromZero);
                }
                
            }
        }
        /// <summary>
        /// 应收未税金额
        /// </summary>
        public decimal? ReceiptsAmount { get; set; }
        /// <summary>
        /// 应收含税金额
        /// </summary>
        public decimal ReceiptsTaxedAmount
        {
            get
            {
                if (this.ReceiptsAmount != null)
                {
                    if (this.Currency == 0)
                    {
                        return Math.Round(this.ReceiptsAmount.Value * 0.16m,2);
                    }
                    else
                    {
                        return Math.Round(this.ReceiptsAmount.Value,2);
                    }
                }
                else
                {
                    return 0;
                }
               
            }
        }
        /// <summary>
        /// 账单币种
        /// </summary>
        public int? Currency { get; set; }
        /// <summary>
        /// 欠款
        /// </summary>
        public decimal OwedMoney
        {
            get
            {
                if (this.ReceiptsTaxedAmount != 0)
                {
                    return this.PayableTaxedAmount  - this.ReceiptsTaxedAmount; 
                }
                else
                {
                    return this.PayableTaxedAmount;
                }
               
            }
        }
        public string AdminName { get; set; }
        public string payCompanyName { get; set; }
        public string recCompanyName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
