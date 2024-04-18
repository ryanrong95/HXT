using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 日程安排类型
    /// </summary>
    public enum ScheduleType
    {
        /// <summary>
        /// 公有
        /// </summary>
        [Description("公有")]
        Public = 1,
        /// <summary>
        /// 私有
        /// </summary>
        [Description("私有")]
        Private = 2,
    }
}