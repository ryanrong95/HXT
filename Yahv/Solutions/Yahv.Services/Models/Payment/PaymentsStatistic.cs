using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class PaymentsStatistic
    {
        public string OrderID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }
        public string PayableID { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string Payee { get; set; }
        public string Business { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        public Currency Currency { get; set; }
        public string CurrencyName => this.Currency.GetDescription();
        public decimal LeftPrice { get; set; }
        public DateTime LeftDate { get; set; }
        public decimal? RightPrice { get; set; }
        public DateTime? RightDate { get; set; }
        /// <summary>
        /// 剩余应付
        /// </summary>
        public decimal Remains
        {
            get
            {
                return this.LeftPrice - (this.RightPrice ?? 0m) - (this.ReducePrice ?? 0m);
            }
        }

        public string Summay { get; set; }

        public string AdminID { get; set; }
        public string AdminName { get; set; }

        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 减免金额
        /// </summary>
        public decimal? ReducePrice { get; set; }

        /// <summary>
        /// 付款人匿名
        /// </summary>
        public string PayerAnonymous { get; set; }

        /// <summary>
        /// 收款人匿名
        /// </summary>
        public string PayeeAnonymous { get; set; }

        /// <summary>
        /// 账单ID
        /// </summary>
        public string VoucherID { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string TrackingNumber { get; set; }

        #region 扩展属性
        public CenterFileDescription[] Files { get; set; }

        #endregion
    }
}
