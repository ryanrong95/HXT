using Yahv.Underly.Attributes;

namespace Yahv.Services
{
    /// <summary>
    /// 系统业务
    /// </summary>
    public enum SysBusiness
    {
        [Description("采购业务")]
        Purchases,
        [Description("询报价")]
        RFQ,
        [Description("库房管理")]
        PfWms,
        [Description("Erm")]
        Erm,
        [Description("CRM")]
        CRM,
        [Description("SRM")]
        SRM
    }
}
