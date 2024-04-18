using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 付汇申请状态
    /// </summary>
    public enum PayExchangeApplyStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Auditing = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,

        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Approvaled = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancled = 4,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 5
    }
}