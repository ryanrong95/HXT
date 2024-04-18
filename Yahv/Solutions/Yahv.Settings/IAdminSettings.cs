namespace Yahv.Settings
{
    /// <summary>
    /// 管理员配置接口
    /// </summary>
    public interface IAdminSettings : ISettings
    {
        /// <summary>
        /// 超级管理员ID
        /// </summary>
        string SuperID { get; }

        /// <summary>
        /// 登录cookie 名称
        /// </summary>
        string LoginCookieName { get; }

        /// <summary>
        /// 登录 组织机构 名称
        /// </summary>
        string League { get; }

        /// <summary>
        /// 超级管理员登录名
        /// </summary>
        string SuperName { get; }
        /// <summary>
        /// 超级管理员真实名
        /// </summary>
        string SuperRealName { get; set; }

        /// <summary>
        /// 超级角色ID
        /// </summary>
        string SuperRoleID { get; }

        /// <summary>
        /// 超级角色名
        /// </summary>
        string SuperRoleName { get; }
    }
}
