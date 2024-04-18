using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 报关通知状态
    /// </summary>
    public enum DeclareNoticeStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未制单")]
        UnDec = 0,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已制单")]
        AllDec = 1,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 2,
    }
}
