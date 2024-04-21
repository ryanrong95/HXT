using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 月账单状态
    /// </summary>
    public enum PayBillStatus
    {
        /// <summary>
        /// 考核
        /// </summary>
        [Description("考核")]
        Check = 1,

        /// <summary>
        /// 封账
        /// </summary>
        [Description("封账")]
        Closed = 2,

        /// <summary>
        /// 已发放
        /// </summary>
        [Description("已发放")]
        Send = 3,
    }
}