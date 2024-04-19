using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 账户类型 1 现金余额 2 信用
    /// </summary>
    public enum UserAccountType
    {
        /// <summary>
        /// 现金余额
        /// </summary>
        [Description("现金余额")]
        Cash = 1,
        /// <summary>
        /// 信用
        /// </summary>
        [Description("信用")]
        Credit = 2,
        /// <summary>
        /// TT
        /// </summary>
        [Description("TT")]
        TT = 3
    }
}
