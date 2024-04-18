using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Enums
{
    public enum RefundApplyStatus
    {
        /// <summary>
        /// 已提交
        /// </summary>
        [Description("已提交")]
        Applied = 1,

        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Approved = 2,

        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("已拒绝")]
        Rejected = 3,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Paid = 4,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 5,
    }
}
