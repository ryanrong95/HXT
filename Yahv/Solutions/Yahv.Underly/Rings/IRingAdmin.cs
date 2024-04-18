namespace Yahv.Underly.Rings
{
    /// <summary>
    /// Yahv.Erp的Admin接口
    /// </summary>
    public interface IRingAdmin
    {
        /// <summary>
        /// Erp 管理员 只读 ID
        /// </summary>
        string ID { get; }
        /// <summary>
        /// Erp 管理员 只读 登录名
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Erp 管理员 只读 真实姓名
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// 是否系统超级管理员
        /// </summary>
        bool IsSuper { get; }

        /// <summary>
        /// 角色
        /// </summary>
        IRingRole Role { get; }
    }
}
