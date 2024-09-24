using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 数据来源
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// 线下
        /// </summary>
        [Description("线下")]
        Offline = 0,
        /// <summary>
        /// 线上
        /// </summary>
        [Description("线上")]
        Online = 1,
        /// <summary>
        /// b1b
        /// </summary>
        [Description("b1b")]
        B1b = 2,
        /// <summary>
        /// 华芯通
        /// </summary>
        [Description("华芯通")]
        Xdt = 3,
    }

}
