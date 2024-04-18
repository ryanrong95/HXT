using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 货期
    /// </summary>
    public enum TradeType
    {
        /// <summary>
        /// 不限
        /// </summary>
        [Description("不限")]
        Unknown = 0,
        /// <summary>
        /// 现货
        /// </summary>
        [Description("现货")]
        Present = 1,
        /// <summary>
        /// 期货
        /// </summary>
        [Description("期货")]
        Futures = 2
    }

}
