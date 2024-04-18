using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 费用申请类型
    /// </summary>
    public enum CostApplyType
    {
        /// <summary>
        /// 正常费用申请
        /// </summary>
        [Description("正常")]
        Normal = 1,

        /// <summary>
        /// 特殊费用申请
        /// </summary>
        [Description("特殊")]
        Special = 2,
    }
}
