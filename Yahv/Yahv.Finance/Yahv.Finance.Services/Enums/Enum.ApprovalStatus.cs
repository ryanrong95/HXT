using System;
using System.Runtime.InteropServices;
using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 审批结果
    /// </summary>
    public enum ApprovalStatus
    {
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Submit = 0,
        /// <summary>
        /// 同意
        /// </summary>
        [Description("同意")]
        Agree = 1,
        /// <summary>
        /// 驳回
        /// </summary>
        [Description("不同意")]
        Reject = 2,

        /// <summary>
        /// 付款
        /// </summary>
        [Description("付款")]
        Payment = 100
    }
}