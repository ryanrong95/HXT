
using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 报关单税费类型
    /// </summary>
    public enum DecTaxType
    {
        [Description("进口关税")]
        Tariff = 1,

        [Description("进口增值税")]
        AddedValueTax = 2,

        [Description("消费税")]
        ConsumptionTax = 3,
    }
}
