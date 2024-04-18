using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 归类状态
    /// </summary>
    public enum ClassifyStatus
    {
        /// <summary>
        /// 未归类
        /// </summary>
        [Description("未归类")]
        Unclassified,

        /// <summary>
        /// 首次归类
        /// </summary>
        [Description("首次归类")]
        First,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        Anomaly,

        /// <summary>
        /// 归类完成
        /// </summary>
        [Description("归类完成")]
        Done
    }
}
