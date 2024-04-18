using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 日期类型
    /// </summary>
    public enum CalendarType
    {
        /// <summary>
        /// 工作日
        /// </summary>
        [Description("工作日")]
        WorkingDay = 0,
        /// <summary>
        /// 公休日
        /// </summary>
        [Description("公休日")]
        Holiday = 1,
        /// <summary>
        /// 元旦节
        /// </summary>
        [Description("元旦")]
        NewYearsDay = 2,
        /// <summary>
        /// 春节
        /// </summary>
        [Description("春节")]
        SpringFestival = 3,
        /// <summary>
        /// 清明节
        /// </summary>
        [Description("清明节")]
        TombSweepingDay = 4,
        /// <summary>
        /// 劳动节
        /// </summary>
        [Description("劳动节")]
        LabourDay = 5,
        /// <summary>
        /// 端午节
        /// </summary>
        [Description("端午节")]
        DragonBoatFestival = 6,
        /// <summary>
        /// 中秋节
        /// </summary>
        [Description("中秋节")]
        MidAutumnFestival = 7,
        /// <summary>
        /// 国庆节
        /// </summary>
        [Description("国庆节")]
        NationalDay = 8,
        /// <summary>
        /// 国庆中秋节
        /// </summary>
        //[Description("国庆中秋节")]
        //MidAutuFestivalAndNationalDay = 9,
    }
}