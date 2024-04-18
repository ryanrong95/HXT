using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 付汇状态
    /// </summary>
    public enum PayExchangeStatus
    {
        [Description("未付汇")]
        UnPay = 1,

        [Description("部分付汇")]
        Partial = 2,

        [Description("已付汇")]
        All = 4,
    }
}