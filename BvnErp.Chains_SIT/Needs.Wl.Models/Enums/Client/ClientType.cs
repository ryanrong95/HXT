using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 客户类型
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        /// 自有公司
        /// </summary>
        [Description("A类客户")]
        Internal = 1,

        /// <summary>
        /// 外单客户
        /// </summary>
        [Description("B类客户")]
        External = 2
    }
}