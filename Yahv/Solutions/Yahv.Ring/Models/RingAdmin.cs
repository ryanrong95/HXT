
using Layers.Data.Sqls;
using System;
using Layers.Linq;
using Yahv.Settings;
using System.IO;
using Yahv.Underly.Rings;

namespace Yahv.Models
{
    /// <summary>
    /// Erp系统管理员
    /// </summary>
    public partial class RingAdmin : IRingAdmin, Linq.IUnique
    {
        /// <summary>
        /// 构造函数，初始化对象
        /// </summary>
        internal RingAdmin()
        {

        }

        /// <summary>
        /// 模拟cookie 地址
        /// </summary>
        static public readonly string CookiePath;

        static RingAdmin()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tcoc");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //CookiePath = Path.Combine(path,$"{Guid.NewGuid().ToString("N")}.cookie");
            CookiePath = Path.Combine(path, $"Ring.cookie");
        }

        #region 属性

        /// <summary>
        /// Erp 管理员 只读 ID
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// Erp 管理员 只读 登录名
        /// </summary>
        public string UserName { get; internal set; }
        /// <summary>
        /// Erp 管理员 只读 真实名
        /// </summary>
        public string RealName { get; internal set; }

        /// <summary>
        /// 是否系统超级管理员
        /// </summary>
        public bool IsSuper
        {
            get
            {
                if (this.ID == SettingsManager<IAdminSettings>.Current.SuperID
                    || this.UserName == SettingsManager<IAdminSettings>.Current.SuperName)
                //  || 属于超级管理员权限的一般管理员
                {
                    return true;
                }
                else
                {
                    return this.Status == Underly.AdminStatus.Super || this.Role.IsSuper;
                }
            }
        }

        /// <summary>
        /// 角色
        /// </summary>
        public IRingRole Role { get; internal set; }

        /// <summary>
        /// 角色
        /// </summary>
        internal Underly.AdminStatus Status { get; set; }

        #endregion

    }
}
