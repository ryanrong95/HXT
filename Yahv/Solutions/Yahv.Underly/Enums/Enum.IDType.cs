using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 证件状态
    /// </summary>
    public enum IDType
    {
        [Description("身份证")]
        IDCard = 1,

        [Description("驾驶证")]
        IDDriver = 2,

        /// <summary>
        /// 带公章提货
        /// </summary>
        [Description("带公章提货")]
        PickSeal = 3,
    }
}
