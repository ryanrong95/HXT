using Yahv.Underly.Attributes;

namespace Yahv.Plats.Services
{
    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 正常
        /// </summary>

        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 关闭、停用(可见不可用，暂时开发为不可见不可用)
        /// </summary>
        [Description("停用", "关闭")]
        Closed = 500,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除", "关闭")]
        Delete = 400,
    }
}
