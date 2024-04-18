using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        Customer = 1,

        /// <summary>
        /// 合并
        /// </summary>
        [Description("合并")]
        Compose = 2,
    }
}
