using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    ///// <summary>
    ///// 付款方式
    ///// </summary>
    //public enum PaymentType
    //{
    //    [Description("支票")]
    //    Check = 1,

    //    [Description("现金")]
    //    Cash = 2,

    //    [Description("转账")]
    //    Transfer = 3,

    //    [Description("电汇")]
    //    Exchange = 4,
    //}

    /// <summary>
    /// 货物条款类型
    /// </summary>
    public enum WayChargeType
    {
        [Description("代付货款")]
        PayCharge = 1,

        [Description("代收货款")]
        ReciveCharge = 2,
    }
}
