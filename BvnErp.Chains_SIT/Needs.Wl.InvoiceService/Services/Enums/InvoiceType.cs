using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Enums
{
    /// <summary>
    /// 开票类型
    /// </summary>
    public enum InvoiceType
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
}
