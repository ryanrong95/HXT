using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 财务通知类型
    /// </summary>
    public enum VoucherType
    {
        /// <summary>
        /// 收款确认
        /// </summary>
        [Description("收款确认")]
        Receipt = 10,

        /// <summary>
        /// 付款确认
        /// </summary>
        [Description("付款确认")]
        Payment = 20,

        /// <summary>
        /// 还款确认
        /// </summary>
        [Description("还款确认")]
        Repay = 30,
    }
}
