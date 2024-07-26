using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SwapDetail : IUnique
    {
        public string ID { get; set; }
        public string SwapNoticeID { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 币种 SwapNotices Currency
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 换汇金额 SwapNoticeItems  Amount
        /// </summary>
        public decimal? SwapAmount { get; set; }
        /// <summary>
        /// 报关当日汇率 Orders RealExchangeRate
        /// </summary>
        public decimal? RealExchangeRate { get; set; }
       
        /// <summary>
        /// 运保杂 外币金额*0.002
        /// </summary>
        public decimal? TransPremiumInsuranceAmount
        {
            get
            {
                return SwapAmount * 0.002M;
            }
        }
        /// <summary>
        /// 运保杂RMB
        /// </summary>
        public decimal? TransPremiumInsuranceAmountRMB
        {
            get
            {
                return TransPremiumInsuranceAmount * RealExchangeRate;
            }
        }
        /// <summary>
        /// 实际换汇汇率 SwapNotices ExchangeRate
        /// </summary>
        public decimal? SwapExchangeRate { get; set; }
        /// <summary>
        /// 汇兑-客户(借)
        /// </summary>
        public decimal? AcceptanceCustomer
        {
            get
            {
                decimal swapAmount = this.SwapAmount == null ? 0 : this.SwapAmount.Value;
                return swapAmount * (SwapExchangeRate - RealExchangeRate);
            }
        }
        /// <summary>
        /// 汇兑-华芯通（借）
        /// </summary>
        public decimal? AcceptanceXDT
        {
            get
            {
                return TransPremiumInsuranceAmount * (SwapExchangeRate - RealExchangeRate);
            }
        }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

    }
}
