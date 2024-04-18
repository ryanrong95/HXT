using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 开票通知状态
    /// </summary>
    public enum InvoiceNoticeStatus
    {
        [Description("待开票")]
        UnAudit = 1,

        [Description("开票中")]
        Auditing = 2,

        [Description("已完成")]
        Confirmed = 3,

        [Description("已取消")]
        Canceled = 4
    }
}
