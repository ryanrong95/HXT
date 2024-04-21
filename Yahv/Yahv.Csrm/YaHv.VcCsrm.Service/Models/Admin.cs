using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.VcCsrm.Service.Models
{
    public class Admin : IUnique
    {
        /// <summary>
        /// 唯一码:Adm001
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 登入名称
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? LastLoginDate { get; internal set; }

        /// <summary>
        /// RoleID
        /// </summary>
        public string RoleID { get; internal set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; internal set; }

    }
}
