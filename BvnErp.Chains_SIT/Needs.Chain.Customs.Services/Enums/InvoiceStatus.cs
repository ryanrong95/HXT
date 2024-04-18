using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 代理订单开票状态
    /// </summary>
    public enum InvoiceStatus
    {
        [Description("未开票")]
        UnInvoiced = 1,

        [Description("已申请")]
        Applied = 2,

        [Description("已开票")]
        Invoiced = 3
    }
}
