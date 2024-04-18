using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 条款类型
    /// </summary>
    public enum ClauseType
    {
        /// <summary>
        /// 代理费率
        /// </summary>
        [Description("代理费率")]
        AgencyFee = 1,

        /// <summary>
        /// 付汇条款
        /// </summary>
        [Description("付汇条款")]
        Exchange = 2,

        /// <summary>
        /// 开票条款
        /// </summary>
        [Description("开票条款")]
        ExchangeRate = 3,

        /// <summary>
        /// 结算方式
        /// </summary>
        [Description("结算方式")]
        Finiancial = 4,

        /// <summary>
        /// 结算汇率
        /// </summary>
        [Description("结算汇率")]
        Invoice = 5,

        /// <summary>
        /// 协议期限
        /// </summary>
        [Description("协议期限")]
        AgreementTerm = 6,
    }

    /// <summary>
    /// 账期类型
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        PrePaid = 0,

        /// <summary>
        /// 约定期限
        /// </summary>
        [Description("约定期限")]
        AgreedPeriod = 1,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        Monthly = 2,
    }

    /// <summary>
    /// 开票类型
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 全额发票
        /// </summary>
        [Description("全额发票")]
        Full = 0,

        /// <summary>
        /// 服务费发票
        /// </summary>
        [Description("服务费发票")]
        Service = 1,
    }

    /// <summary>
    /// 服务费开票税率
    /// </summary>
    public enum InvoiceRate
    {
        /// <summary>
        /// 3%
        /// </summary>
        [Description("3%")]
        ThreePercent = 3,

        /// <summary>
        /// 6%
        /// </summary>
        [Description("6%")]
        SixPercent = 6,
    }

    /// <summary>
    /// 汇率类型
    /// </summary>
    public enum ExchangeRateType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 海关汇率
        /// </summary>
        [Description("海关汇率")]
        Custom = 1,

        /// <summary>
        /// 实时汇率
        /// </summary>
        [Description("实时汇率")]
        RealTime = 2,

        /// <summary>
        /// 约定汇率
        /// </summary>
        [Description("约定汇率")]
        Agreed = 3,

        /// <summary>
        /// 九点半汇率
        /// </summary>
        [Description("九点半汇率")]
        NineRealTime = 4,
    }
}
