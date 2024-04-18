using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 注册申请状态
    /// </summary>
    public enum HandleStatus
    {
        [Description("待处理")]
        Pending = 0,

        [Description("已处理")]
        Processed = 1,
    }
}