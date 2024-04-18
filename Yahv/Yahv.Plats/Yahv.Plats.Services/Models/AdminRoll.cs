using Yahv.Linq;
using System;
using Yahv.Settings;

namespace Yahv.Plats.Services.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class AdminRoll : IUnique
    {
        /// <summary>
        /// 唯一码:Adm001
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }
        /// <summary>
        /// 登入名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 登入密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 默认使用：ID的数字部分。即，001
        /// </summary>
        public string SelCode { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public RoleRoll Role { get; internal set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 管理员（在系统中要时刻显示管理员的在职与离职状态）：超级管理员、正常（管理员）、停用
        /// </summary>
        public Underly.AdminStatus Status { get; set; }
        /// <summary>
        /// 最终登入时间
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// 密码修改日期
        /// </summary>
        public DateTime? PwdModifyDate { get; set; }

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


        internal AdminRoll()
        {

        }
    }
}