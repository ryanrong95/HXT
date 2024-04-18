using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 日程安排来源
    /// </summary>
    public enum ScheduleFrom
    {
        /// <summary>
        /// 政府
        /// </summary>
        [Description("国家")]
        Government = 1,
        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company = 2,
    }
}