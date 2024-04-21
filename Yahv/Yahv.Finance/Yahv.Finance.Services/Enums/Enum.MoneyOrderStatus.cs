using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 承兑汇票状态
    /// </summary>
    public enum MoneyOrderStatus
    {
        /// <summary>
        /// 已签收
        /// </summary>
        //[Description("已签收")]
        //Saved = 10,

        /// <summary>
        /// 未兑换
        /// </summary>
        [Description("未兑换")]
        Ticketed = 20,

        /// <summary>
        /// 已兑换
        /// </summary>
        [Description("已兑换")]
        Exchanged = 200,

        /// <summary>
        /// 已转让
        /// </summary>
        [Description("已转让")]
        Transferred = 201,
    }
}