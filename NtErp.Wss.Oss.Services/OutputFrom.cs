using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 账户支出去向
    /// </summary>
    public enum OutputTo
    {
        /// <summary>
        /// 支出
        /// </summary>
        [Description("支付")]
        Pay = 1,
        /// <summary>
        /// 信用还款
        /// </summary>
        [Description("信用还款")]
        Repay = 2,
      
    }
}
