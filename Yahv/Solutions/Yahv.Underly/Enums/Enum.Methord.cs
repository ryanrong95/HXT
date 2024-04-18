using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 汇款方式
    /// </summary>
    public enum Methord
    {
        [Description("TT")]
        TT = 1,

        [Description("支付宝")]
        Alipay = 2,

        [Description("转账")]
        Transfer = 3,

        [Description("电汇")]
        Exchange = 4,

        [Description("支票")]
        Check = 5,

        [Description("现金")]
        Cash = 6,
    }
}
