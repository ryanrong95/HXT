using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 开票申请状态
    /// </summary>
    public enum InvoiceEnum
    {
        /// <summary>
        /// 未开票
        /// </summary>
        [Description("未开票")]
        UnInvoiced = 1,

        /// <summary>
        /// 开票中
        /// </summary>
        [Description("开票中")]
        Applied = 2,

        /// <summary>
        /// 已开票
        /// </summary>
        [Description("已开票")]
        Invoiced = 3,
    }

    /// <summary>
    /// 开票申请来源
    /// </summary>
    public enum InvoiceFromType
    {
        /// <summary>
        /// 香港代仓储
        /// </summary>
        [Description("香港代仓储")]
        HKStore = 1,

        /// <summary>
        /// 深圳代仓储
        /// </summary>
        [Description("深圳代仓储")]
        SZStore = 2,
    }
}
