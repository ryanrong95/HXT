using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 报关单缴税状态
    /// </summary>
    public enum DecTaxStatus
    {
        [Description("未缴税")]
        Unpaid = 1,

        [Description("已缴税")]
        Paid = 2,

        [Description("已抵扣")]
        Deducted = 3,
    }
}
