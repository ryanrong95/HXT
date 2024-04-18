namespace Yahv.Settings
{
    /// <summary>
    /// 询报价配置
    /// </summary>
    public interface IRFQSettings : ISettings
    {
        /// <summary>
        /// 供应商报价上限个数
        /// </summary>
        int LimitSupplierCount { get; }
        /// <summary>
        /// 询价无报价自动失效时间间隔(天)
        /// </summary>
        int InquiryAbandonTimespan { get; set; }

        /// <summary>
        /// 搜索条件开始日期间隔(月)
        /// </summary>
        int SearchTimeSpan { get; set; }
    }
}
