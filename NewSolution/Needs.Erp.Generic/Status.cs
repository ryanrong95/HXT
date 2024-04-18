using Needs.Utils.Descriptions;

namespace Needs.Erp.Generic
{
    /// <summary>
    /// 通用状态
    /// </summary>
    public enum Status
    {
        [Description("待审批")]
        Auditing = 100,
        [Description("正常")]
        Normal = 200,
        [Description("删除")]
        Delete = 400
    }

    /// <summary>
    /// 权限状态
    /// </summary>
    public enum RoleStatus
    {
        Normal = 200,

        /// <summary>
        /// 超级管理员[权限]
        /// </summary>
        Super = 210,

        Delete = 400
    }
}
