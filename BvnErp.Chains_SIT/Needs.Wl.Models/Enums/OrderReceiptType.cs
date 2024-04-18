using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 订单应收/实收类型
    /// </summary>
    public enum OrderReceiptType
    {
        /// <summary>
        /// 应收
        /// </summary>
        [Description("应收")]
        Receivable = 1,

        /// <summary>
        /// 实收
        /// </summary>
        [Description("实收")]
        Received = 2
    }
}
