using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FeeReport
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public Client Client { get; set; }
        public DateTime Date { get; set; }
        public string OrderID { get; set; }
        public OrderPremiumType Type { get; set; }
        /// <summary>
        /// 实际应收未税金额 OrderPremiums 里 UnitPrice的金额 (应付金额)
        /// </summary>
        public decimal PayableUnitPrice { get; set; }
        /// <summary>
        /// 实际应收的数量(应付) OrderPremiums 里 Count
        /// </summary>
        public decimal PayableCount { get; set; }
        /// <summary>
        /// 实际应收的数量(应付) OrderPremiums 里 Currency
        /// </summary>
        public string PayableCurrency { get; set; }
        /// <summary>
        /// 实际应收未税金额(应付未税金额)
        /// </summary>
        public decimal PayableAmount
        {
            get
            {
                return this.PayableUnitPrice * this.PayableCount;
            }
        }

        /// <summary>
        /// 实际应收含税金额 OrderReceipts 里 type为1 的金额(应付含税金额)
        /// </summary>
        public decimal PayableTaxedAmount { get; set; }

        /// <summary>
        /// 应收金额 OrderPremiums 里的 standardPrice (应收未税金额)
        /// </summary>
        public decimal? ReceivableAmount { get; set; }

        /// <summary>
        /// 应收金额 OrderPremiums 里的 standardPrice (应收未税金额)
        /// </summary>
        public string ReceivableCurrency { get; set; }
        /// <summary>
        /// 实收金额 OrderReceipts 里 type为2 的金额
        /// </summary>
        public decimal? ReceiptsAmount { get; set; }
        public decimal? exchangeRate  { get; set; }
        /// <summary>
        /// 欠款
        /// </summary>
        public decimal OwedMoney
        {
            get
            {
                return this.ReceiptsAmount == null ?  this.PayableTaxedAmount : this.PayableTaxedAmount  - this.ReceiptsAmount.Value;
            }
        }
        /// <summary>
        /// 费用优惠
        /// </summary>
        public decimal Discount
        {
            get
            {
                decimal discount = 0;
                if(this.ReceivableCurrency!= this.PayableCurrency)
                {                  
                    if (exchangeRate != null)
                    {
                        discount = this.ReceivableAmount == null ? 0 : this.ReceivableAmount.Value * exchangeRate.Value - this.PayableAmount;
                    }                  
                }
                else
                {
                    discount = this.ReceivableAmount == null ? 0 : this.ReceivableAmount.Value - this.PayableAmount;
                }
                return Math.Round(discount,2);
            }
        }
        /// <summary>
        /// 费用录入人
        /// </summary>
        public Admin FeeCreator { get; set; }
        public string FeeCreatorID { get; set; }
    }
}
