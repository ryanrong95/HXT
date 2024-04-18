using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 时间段分类
    /// </summary>
    [Flags]
    public enum TimeCatalog
    {
        /// <summary>
        /// 上午
        /// </summary>
        [Description("上午")]
        AM = 1,
        /// <summary>
        /// 下午
        /// </summary>
        [Description("下午")]
        PM = 2,
    }
}
