using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 费用类型
    /// </summary>
    public enum FeeType
    {
        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        Product = 1,

        /// <summary>
        /// 税款
        /// </summary>
        [Description("税款")]
        Tax = 2,

        /// <summary>
        /// 代理费
        /// </summary>
        [Description("代理费")]
        AgencyFee = 3,

        /// <summary>
        /// 杂费
        /// </summary>
        [Description("杂费")]
        Incidental = 4,
    }
}