namespace Yahv.Underly
{
    /// <summary>
    /// 附加费类型
    /// </summary>
    public enum PremiumType
    {
        /// <summary>
        /// 额外关税
        /// </summary>
        [Attributes.Description("额外关税")]
        ExtraDuties = 1,
        /// <summary>
        /// 商检费
        /// </summary>
        [Attributes.Description("商检费")]
        Inspection = 2,
        /// <summary>
        /// 运费
        /// </summary>
        [Attributes.Description("运费")]
        Freight = 3,
        /// <summary>
        /// 其他
        /// </summary>
        [Attributes.Description("其他")]
        Other = 999
    }
}
