using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 供应商类型
    /// </summary>
    [Flags]
    public enum SupplierType
    {
        /// <summary>
        /// 不限
        /// </summary>
        [Description("不限")]
        Unknown = 0,

        /// <summary>
        /// 市场
        /// </summary>
        [Description("市场")]
        Market = 1,
        /// <summary>
        /// 海外
        /// </summary>
        [Description("海外")]
        Overseas = 2 ,
        /// <summary>
        /// 高端
        /// </summary>
        [Description("高端")]
        TopGrade = 2 << 1,
        /// <summary>
        /// 固定渠道
        /// </summary>
        [Description("固定渠道")]
        Fixed = 2 << 2,
    }
}
