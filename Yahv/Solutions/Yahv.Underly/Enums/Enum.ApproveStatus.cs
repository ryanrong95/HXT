using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 审批状态
    /// </summary>
    public enum ApprovalStatus
    {
        /// <summary>
        /// 等待,待审核
        /// </summary>
        [Description("未完善")]
        UnComplete = 1,
        //等待：waiting, 否决：voted, 正常：Normal, 黑名单：black, 停用：closed, 删除：Deleted
        /// <summary>
        /// 等待,待审核
        /// </summary>
        [Description("待审核")]
        Waitting = 100,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,
        /// <summary>
        /// 否决
        /// </summary>
        [Description("否决")]
        Voted = 300,

        /// <summary>
        /// 黑名单
        /// </summary>
        [Description("黑名单")]
        Black = 400,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Closed = 500,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 600,
    }
}
