using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Enums
{
    /// <summary>
    /// TaxMapApiStatus 
    /// </summary>
    public enum TaxMapApiStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        UnHandled = 1,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 2,

        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error = 3,

        /// <summary>
        /// 未匹配到TaxManage
        /// </summary>
        [Description("未匹配到TaxManage")]
        UnMatchManager = 4,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Exception = 5,

        /// <summary>
        /// 未处理(收到的发票)
        /// </summary>
        [Description("未处理(收到的发票)")]
        RevUnHandled = 11,
    }
}
