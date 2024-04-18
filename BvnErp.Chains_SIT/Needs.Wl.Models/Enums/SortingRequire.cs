using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 分拣要求
    /// </summary>
    public enum SortingRequire
    {
        /// <summary>
        /// 不拆箱
        /// </summary>
        [Description("不拆箱")]
        Packed = 1,

        /// <summary>
        /// 拆箱
        /// </summary>
        [Description("拆箱")]
        UnPacking = 2,
    }
}
