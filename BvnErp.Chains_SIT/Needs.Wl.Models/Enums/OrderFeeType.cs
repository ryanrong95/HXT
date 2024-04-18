using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 订单收款费用类型
    /// </summary>
    public enum OrderFeeType
    {
        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        Product = 1,

        /// <summary>
        /// 关税
        /// </summary>
        [Description("关税")]
        Tariff = 2,

        /// <summary>
        /// 增值税
        /// </summary>
        [Description("增值税")]
        AddedValueTax = 3,

        /// <summary>
        /// 代理费
        /// </summary>
        [Description("代理费")]
        AgencyFee = 4,

        /// <summary>
        /// 杂费
        /// </summary>
        [Description("杂费")]
        Incidental = 5,
    }
}