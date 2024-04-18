using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 通用状态
    /// </summary>
    public enum ContactType
    {
        /// <summary>
        /// 线上
        /// </summary>
        [Description("线上")]
        Online = 1,
        /// <summary>
        /// 线下
        /// </summary>
        [Description("线下")]
        Offline = 2,
        /// <summary>
        /// 销售
        /// </summary>
        [Description("销售")]
        Sales = 3,
        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        Pruchaser = 4
    }
}
