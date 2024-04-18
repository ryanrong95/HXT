using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PvRoute.Services
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
        /// 第三方付
        /// </summary>
        [Description("第三方付")]
        ThirdPay = 3,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        MonthlyPay = 4
    }

    /// <summary>
    /// 跨越支付方式（通用支付方式转化）
    /// </summary>
    /// <remarks>
    /// 当付款方式=10-寄方付 或者 30-第三方付 时，付款账号必填
    /// </remarks>
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
        /// 第三方付
        /// </summary>
        [Description("第三方付")]
        ThirdPay = 30
    }

    public enum PrintSource
    {
        [Description("顺丰")]
        SF = 10,
        [Description("跨越速运")]
        KY = 20,
        [Description("EMS")]
        EMS =30

    }

    public enum PrintState
    {
        [Description("正常的")]
        Normal = 10,
        [Description("无效的")]
        Waiting = 20
    }
}
