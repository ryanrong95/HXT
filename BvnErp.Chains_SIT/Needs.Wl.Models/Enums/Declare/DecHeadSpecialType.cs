using System.ComponentModel;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 报关单特殊类型
    /// </summary>
    public enum DecHeadSpecialType
    {
        /// <summary>
        /// 包车
        /// </summary>
        [Description("包车")]
        CharterBus = 1,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue = 2,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 3,

        /// <summary>
        /// 检疫
        /// </summary>
        [Description("检疫")]
        Quarantine = 4,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 5,
    }
}
