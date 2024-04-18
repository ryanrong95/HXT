using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.Print.Library
{
    /// <summary>
    /// 顺丰支付方式
    /// </summary>
    public enum SFPayType
    {
        /// <summary>
        /// 寄付
        /// </summary>
        [Description("寄付")]
        DeliveryPay = 1,

        /// <summary>
        /// 到付
        /// </summary>
        [Description("到付")]
        CollectPay = 2,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("第三方付")]
        MonthlyPay = 3
    }

    /// <summary>
    /// 跨越支付方式（通用支付方式转化）
    /// </summary>
    public enum KYPayType
    {
        /// <summary>
        /// 寄付
        /// </summary>
        [Description("寄付")]
        DeliveryPay = 10,

        /// <summary>
        /// 到付
        /// </summary>
        [Description("到付")]
        CollectPay = 20,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("第三方付")]
        MonthlyPay = 30
    }

    public enum PrintSource
    {
        [Description("顺丰")]
        SF = 10,
        [Description("跨越速运")]
        KYSY = 20
    }
}
