using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 入库通知状态
    /// </summary>
    public enum EntryNoticeStatus
    {
        /// <summary>
        /// 待装箱\待入库
        /// </summary>
        [Description("待装箱")]
        UnBoxed = 1,

        /// <summary>
        /// 已装箱\已入库
        /// </summary>
        [Description("已装箱")]
        Boxed = 2,

        /// <summary>
        /// 已封箱
        /// </summary>
        [Description("已封箱")]
        Sealed = 3
    }

    /// <summary>
    /// 分拣状态
    /// </summary>
    public enum SortingStatus
    {
        /// <summary>
        /// 等待装箱
        /// </summary>
        [Description("等待装箱")]
        Waiting = 1,

        /// <summary>
        /// 分拣\装箱完成
        /// </summary>
        [Description("装箱完毕")]
        Sorted = 2,

        /// <summary>
        /// 分拣异常
        /// </summary>
        [Description("分拣异常")]
        Exception = 3
    }
}