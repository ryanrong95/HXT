using System;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
    [ViewFactory(typeof(Views.AdminsAll<>))]
    public class Admin : IUnique
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
        /// 默认使用：ID的数字部分。即，001
        /// </summary>
        public string SelCode { get; set; }


        /// <summary>
        /// 状态 正常（管理员：在系统中要时刻显示管理员的在职与离职状态）、停用、超级管理员。
        /// </summary>
        public AdminStatus Status { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 华芯通AdminID
        /// </summary>
        public string OriginID { get; set; }
    }
}
