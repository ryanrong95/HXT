using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 订单管控状态
    /// </summary>
    public enum OrderControlStatus
    {
        /// <summary>
        /// 当前管控步骤待审核
        /// </summary>
        [Description("待审批")]
        Auditing = 100,

        /// <summary>
        /// 当前管控步骤审批通过
        /// </summary>
        [Description("通过")]
        Approved = 200,

        /// <summary>
        /// 当前管控步骤审批未通过
        /// </summary>
        [Description("拒绝")]
        Rejected = 300
    }
}
