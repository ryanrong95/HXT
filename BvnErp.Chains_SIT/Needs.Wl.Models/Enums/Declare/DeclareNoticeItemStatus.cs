using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 报关通知项状态
    /// </summary>
    public enum DeclareNoticeItemStatus
    {
        /// <summary>
        /// 未制单
        /// </summary>
        [Description("未制单")]
        UnMake = 0,

        /// <已制单>
        /// 已制单
        /// </summary>
        [Description("已制单")]
        Make = 2,

        /// <已制单>
        /// 已制单
        /// </summary>
        [Description("已取消")]
        Cancel = 3
    }
}
