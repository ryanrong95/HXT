using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum DataStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,
        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Closed = 500
    }
}
