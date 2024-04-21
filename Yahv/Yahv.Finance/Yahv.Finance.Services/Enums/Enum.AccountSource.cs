using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 账户来源
    /// </summary>
    public enum AccountSource
    {
        /// <summary>
        /// 标准
        /// </summary>
        [Description("标准")]
        Standard = 1,
        /// <summary>
        /// 简易
        /// </summary>
        [Description("简易")]
        Simple = 2
    }
}