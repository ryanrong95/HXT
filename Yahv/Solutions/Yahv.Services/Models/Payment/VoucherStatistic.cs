using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class VoucherStatistic
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
        //public string ReceivedID { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 付款人账户ID
        /// </summary>
        public string PayerID { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string Payee { get; set; }
        /// <summary>
        /// 收款人账户ID
        /// </summary>
        public string PayeeID { get; set; }

        public string Business { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        /// <summary>
        /// 录入币种
        /// </summary>
        public Currency OriginCurrency { get; set; }
        /// <summary>
        /// 录入币种名称
        /// </summary>
        public string OriginCurrencyName => OriginCurrency.GetDescription();

        /// <summary>
        /// 录入金额
        /// </summary>
        public decimal OriginPrice { get; set; }
        /// <summary>
        /// 结算币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 结算币种名称
        /// </summary>
        public string CurrencyName => Currency.GetDescription();
        /// <summary>
        /// 结算金额
        /// </summary>
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

        public int? OriginalIndex { get; set; }
        public int? ChangeIndex { get; set; }

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

        /// <summary>
        /// 描述
        /// </summary>
        public string Summay { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string CouponID { get; set; }

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
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 型号ID
        /// </summary>
        public string ItemID { get; set; }

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
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// json 数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        #region 扩展属性
        public CenterFileDescription[] Files { get; set; }

        //所有实收记录
        public IEnumerable<Received> Receiveds
        {
            get; set;
        }

        //减免总金额
        public decimal ReduceTotalPrice
        {
            get
            {
                return this.Receiveds?.Where(item => item.AccountType == AccountType.Reduction)?.Sum(item => item.Price) ?? 0;
            }
        }

        //优惠总金额
        public decimal CouponTotalPrice
        {
            get
            {
                return this.Receiveds?.Where(item => item.AccountType == AccountType.Coupon)?.Sum(item => item.Price) ?? 0;
            }
        }

        #endregion
    }
}
