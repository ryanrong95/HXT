
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 商务关联类型
    /// </summary>
    public enum BusinessRelationCategory
    {
        /// <summary>
        /// 血缘
        /// </summary>
        [Description("血缘")]
        Brother = 1,

        /// <summary>
        /// 商务
        /// </summary>
        [Description("商务")]
        Depend = 2,

    }

}
