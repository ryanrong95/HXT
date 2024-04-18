using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.CrmPlus
{
    /// <summary>
    /// 下单方式CrmPlus
    /// </summary>
    public enum OrderType
    {

        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = 0,

        /// <summary>
        /// 网站下单
        /// </summary>
        [Description("网站下单")]
        Website = 1,
        /// <summary>
        /// PO下单
        /// </summary>
        [Description("PO下单")]
        PO = 2,
        /// <summary>
        /// API下单
        /// </summary>
        [Description("API下单")]
        API = 3,

    }
   
}
