using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 角色状态
    /// </summary>
    public enum RoleStatus
    {
        /// <summary>
        /// 正常
        /// </summary>

        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 400,

        /// <summary>
        /// 固定的
        /// </summary>
        [Description("固定的")]
        Fixed = 700,

        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        Super = 900,
    }

    /// <summary>
    /// admin状态
    /// </summary>
    public enum AdminStatus
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
        Closed = 500,

        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        Super = 900,

        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("Npc")]
        Npc = 901,
    }
}
