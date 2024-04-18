namespace Yahv.Underly.Erps
{
    /// <summary>
    /// 权限  Erp 专用 管理员接口
    /// </summary>
    public interface IRoleAdmin
    {

        /// <summary>
        /// 角色
        /// </summary>
        IRole Role { get; }

        /// <summary>
        /// Erp 管理员角色类型
        /// </summary>
        ErpAdminType Type { get; }

        /// <summary>
        /// 返回固定角色枚举
        /// </summary>
        FixedRole? Fixed { get; }

        /// <summary>
        /// 验证是否包涵指定的固定角色
        /// </summary>
        /// <param name="role">固定角色</param>
        /// <returns>包涵结果</returns>
        bool Contains(FixedRole role);
    }
}
