using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    ///品牌关系类型CrmPlus
    /// </summary>
    public enum nBrandType
    {

        /// <summary>
        /// 代理
        /// </summary>
        [Description("代理")]
        Agent = 1,
        /// <summary>
        /// 生产
        /// </summary>
        [Description("生产")]
        Produce = 2,

        /// <summary>
        /// 分销
        /// </summary>
        [Description("分销")]
        Distributor = 3,
    }

}
