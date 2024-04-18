using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 发票类型
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知", "0")]
        Unkonwn = 0,
        /// <summary>
        /// 增值税普通发票
        /// </summary>
        /// <remarks>对应大赢家一般纳税人普票</remarks>
        [Description("增值税普通发票", "2")]
        Normal = 1,
        /// <summary>
        /// 增值税专用发票
        /// </summary>
        /// <remarks>对应大赢家增值税票</remarks>
        [Description("增值税专用发票", "1")]
        VAT = 2,
        /// <summary>
        /// 海关发票
        /// </summary>
        [Description("海关发票", "4")]
        Customs = 3,
        /// <summary>
        /// 普通发票
        /// </summary>
        /// <remarks>对应大赢家普通发票</remarks>
		[Description("普通发票", "3")]
        None = 4
    }
}
