using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    ///申请状态CrmPlus
    /// </summary>
    /// <remarks>
    /// 导入数据可以先导入，再更新
    /// </remarks>
    public enum ProductStatus
    {

        /// <summary>
        /// 机会洽谈
        /// </summary>
        [Description("DO/机会洽谈")]
        DO = 10,

        /// <summary>
        /// 产品导入
        /// </summary>
        [Description("DI/产品导入")]
        DI = 50,

        /// <summary>
        /// 设计采纳
        /// </summary>
        [Description("DW/设计采纳")]
        DW = 80,

        /// <summary>
        /// 批量生产
        /// </summary>
        [Description("MP/批量生产")]
        MP = 100,

        /// <summary>
        /// 机会丢失
        /// </summary>
        [Description("DL/机会丢失")]
        DL = 500,
    }

}
