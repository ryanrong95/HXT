using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 客户状态
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// 未完善
        /// </summary>
        [Description("未完善")]
        Auditing = 1,

        /// <summary>
        /// 已完善
        /// </summary>
        [Description("已完善")]
        Confirmed = 2,
    }
}