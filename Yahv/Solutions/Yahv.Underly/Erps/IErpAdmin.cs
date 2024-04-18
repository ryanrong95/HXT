namespace Yahv.Underly.Erps
{
    /// <summary>
    /// Erp 专用 管理员接口
    /// </summary>
    public interface IErpAdmin   : IRoleAdmin
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
    }
}
