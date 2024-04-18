using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 香港仓库标记已报关，未报关
    /// </summary>
    public enum SortingDecStatus
    {
        /// <summary>
        /// 未报关
        /// </summary>
        [Description("未报关")]
        No = 0,

        /// <已制单>
        /// 已报关
        /// </summary>
        [Description("已报关")]
        Yes = 1
    }
}
