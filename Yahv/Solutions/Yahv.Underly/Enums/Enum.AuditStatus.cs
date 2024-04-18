using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum AuditStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Waiting = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,
        /// <summary>
        /// 否决
        /// </summary>
        [Description("否决")]
        Voted=300,
        /// <summary>
        /// 黑名单
        /// </summary>
        [Description("黑名单")]
        Black =400,
        /// <summary>
        /// 关闭
        /// </summary>
        [Description("停用/禁用")]
        Closed=500,
    }
   
}
