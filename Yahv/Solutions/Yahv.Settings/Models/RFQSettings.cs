using Yahv.Settings.Attributes;

namespace Yahv.Settings.Models
{
    /// <summary>
    /// 询报价设置
    /// </summary>
    class RFQSettings : IRFQSettings
    {
        public RFQSettings()
        {
            LimitSupplierCount = 2;
            InquiryAbandonTimespan = 7;
            SearchTimeSpan = 30;
        }

        [Label("报价供应商上限个数")]
        public int LimitSupplierCount { get; set; }

        [Label("询价无报价自动失效时间间隔(天)")]
        public int InquiryAbandonTimespan { get; set; }

        [Label("搜索条件开始日期间隔(天)")]
        public int SearchTimeSpan { get; set; }
    }
}
