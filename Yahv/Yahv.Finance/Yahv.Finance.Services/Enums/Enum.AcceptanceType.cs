using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 承兑类型
    /// </summary>
    public enum AcceptanceType
    {
        /// <summary>
        /// 贴现
        /// </summary>
        [Description("贴现")]
        Discount = 1,
        /// <summary>
        /// 背书转让
        /// </summary>
        [Description("背书转让")]
        Endorsement = 2,
        /// <summary>
        /// 到期承兑
        /// </summary>
        [Description("到期承兑")]
        Acceptance = 3,
    }
}