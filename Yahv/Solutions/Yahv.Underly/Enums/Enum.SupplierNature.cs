using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 供应商性质(做保留开发)
    /// </summary>
    public enum SupplierNature
    {
        /// <summary>
        /// 国际供应商
        /// </summary>
        [Description("国际供应商")]
        International = 1,
        /// <summary>
        /// 国内供应商
        /// </summary>
        [Description("国内供应商")]
        Domestic = 2,
        /// <summary>
        /// 市场供应商
        /// </summary>
        [Description("市场供应商")]
        Market = 3
    }
}
