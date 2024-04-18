using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PayExchangeDetail : IUnique
    {
        public string ID { get; set; }


        public string PayExchangeApplyID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 报关合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 付汇汇率
        /// </summary>
        public decimal? PayExchangeRate { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 付汇金额 
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 人民币金额 
        /// </summary>
        public decimal? AmountRMB
        {
            get
            {
                return Math.Round(this.Amount.Value * this.PayExchangeRate.Value, 4, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 应收款：整个付汇申请的应收RMB
        /// </summary>
        public decimal? TotalRMB { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string SupplierName { get; set; }
    }
}
