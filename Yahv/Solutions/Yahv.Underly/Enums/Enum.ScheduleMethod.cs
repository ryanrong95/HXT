using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 日程安排方式
    /// </summary>
    public enum ScheduleMethod
    {
        /// <summary>
        /// 工作日
        /// </summary>
        [Description("工作日")]
        Work = 1,
        /// <summary>
        /// 公休日
        /// </summary>
        [Description("公休日")]
        PublicHoliday = 2,
        /// <summary>
        /// 法定节假日
        /// </summary>
        [Description("法定节假日")]
        LegalHoliday = 3,
    }
}