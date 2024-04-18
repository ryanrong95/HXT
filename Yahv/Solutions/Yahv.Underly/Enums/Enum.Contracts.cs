using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 开票类型
    /// </summary>
    public enum BillingType
    {
        /// <summary>
        /// 全额发票
        /// </summary>
        [Description("全额发票")]
        Full = 0,

        /// <summary>
        /// 服务费发票
        /// </summary>
        [Description("服务费发票")]
        Service = 1,
    }

    /// <summary>
    /// 服务费开票税率
    /// </summary>
    public enum InvoiceRate
    {
        /// <summary>
        /// 3%
        /// </summary>
        [Description("3%")]
        ThreePercent = 3,

        /// <summary>
        /// 6%
        /// </summary>
        [Description("6%")]
        SixPercent = 6,
    }
   /// <summary>
    /// 换汇方式
    /// </summary>
    public enum ExchangeMode
    {
        /// <summary>
        /// 预换汇
        /// </summary>
        [Description("预换汇")]
        PrePayExchange = 1,

        /// <summary>
        /// 90天内换汇
        /// </summary>
        [Description("90天内换汇")]
        LimitNinetyDays = 2,

    }
}