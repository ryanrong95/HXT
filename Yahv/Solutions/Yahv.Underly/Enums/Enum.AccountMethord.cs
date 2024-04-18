using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 账户行为
    /// </summary>
    public enum AccountMethord
    {
        /// <summary>
        /// 付款
        /// </summary>
        [Description("付款")]
        Payment = 1,

        /// <summary>
        /// 收款
        /// </summary>
        [Description("收款")]
        Receipt = 2,

        /// <summary>
        /// 调出
        /// </summary>
        [Description("调出")]
        Output = 3,

        /// <summary>
        /// 调入
        /// </summary>
        [Description("调入")]
        Input = 4,

        /// <summary>
        /// 财务费用
        /// </summary>
        [Description("财务费用")]
        Finance = 5,
    }
}
