using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class Bill : Order
    {
        #region 扩展属性
        /// <summary>
        /// 应付款
        /// </summary>
        public decimal? LeftPrice { get; set; }

        /// <summary>
        /// 实付款
        /// </summary>
        public decimal? RightPrice { get; set; }

        /// <summary>
        /// 减免金额
        /// </summary>
        public decimal? ReducePrice { get; set; }

        /// <summary>
        /// 账单创建日期
        /// </summary>
        public DateTime BillCreateDate { get; set; }

        /// <summary>
        /// 剩余应付
        /// </summary>
        public decimal Remains
        {
            get
            {
                return this.LeftPrice.GetValueOrDefault() - this.RightPrice.GetValueOrDefault() - this.ReducePrice.GetValueOrDefault() - this.CouponPrice.GetValueOrDefault();
            }
        }

        /// <summary>
        /// 支付方式： 信用/银行打款
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal? CouponPrice { get; set; }
        #endregion
    }
}
