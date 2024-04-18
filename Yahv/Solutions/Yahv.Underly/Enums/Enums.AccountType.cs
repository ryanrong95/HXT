using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 账户类型
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 信用批复
        /// </summary>
        [Description("信用批复")]
        CreditRecharge = 10,
        /// <summary>
        /// 信用花费
        /// </summary>
        [Description("信用花费")]
        CreditCost = 20,
        /// <summary>
        /// 现金/银行
        /// </summary>
        [Description("现金账户")]
        Cash = 30,
        /// <summary>
        /// 信用总账
        /// </summary>
        [Description("信用总账")]
        CreditTotal = 40,

        /// <summary>
        /// 减免账户
        /// </summary>
        [Description("减免账户")]
        Reduction = 50,

        /// <summary>
        /// 优惠券账户
        /// </summary>
        [Description("优惠券账户")]
        Coupon = 60,

        /// <summary>
        /// 退款账户
        /// </summary>
        [Description("退款账户")]
        Return = 70,

        /// <summary>
        /// 银行流水
        /// </summary>
        [Description("银行流水")]
        BankStatement = 80,
    }
}
