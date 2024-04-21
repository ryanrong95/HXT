using System;
using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 申请状态
    /// </summary>
    public enum ApplyStauts
    {
        /// <summary>
        /// 驳回
        /// </summary>
        [Description("驳回")]
        Rejecting = -10,
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Waiting = 10,
        /// <summary>
        /// 待支付
        /// </summary>
        [Description("待支付")]
        Paying = 20,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 200
    }
}