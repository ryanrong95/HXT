using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class CreditsStatistic
    {
        public string OrderID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 应收ID
        /// </summary>
        public string ReceivableID { get; set; }

        ///// <summary>
        ///// 实收ID
        ///// </summary>
        public string ReceivedID { get; set; }
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
        public decimal LeftPrice { get; set; }
        public DateTime LeftDate { get; set; }
        public decimal? RightPrice { get; set; }
        public DateTime? RightDate { get; set; }

        /// <summary>
        /// 还款日
        /// </summary>
        public DateTime? OriginalDate { get; set; }

        /// <summary>
        /// 还款日（最终）
        /// </summary>
        public DateTime? ChangeDate { get; set; }

        public int OriginalIndex { get; set; }
        public int ChangeIndex { get; set; }

        /// <summary>
        /// 剩余应付
        /// </summary>
        public decimal Remains
        {
            get
            {
                return this.LeftPrice - (this.RightPrice ?? 0m);
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summay { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券金额
        /// </summary>
        public decimal? CouponPrice { get; set; }

        public string AdminID { get; set; }

        public string AdminName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyID { get; set; }

        /// <summary>
        /// 供应商标识
        /// </summary>
        public string SupplierSign { get; set; }
    }
}
