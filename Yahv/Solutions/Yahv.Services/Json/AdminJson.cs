using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Json
{
    public class AdminJson
    {
        /// <summary>
        /// Erp 管理员 只读 ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Erp 管理员 只读 登录名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Erp 管理员 只读 真实名
        /// </summary>
        public string RealName { get; set; }


        public RoleJson Role { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public Underly.AdminStatus Status { get; set; }
    }
}
