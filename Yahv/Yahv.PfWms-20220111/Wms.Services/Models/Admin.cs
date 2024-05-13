using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models
{
    public class Admin
    {
        /// <summary>
        /// 编号 
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 员工编号
        /// </summary>
        public string StaffID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 管理员状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 角色状态
        /// </summary>
        public int? RoleStatus { get; set; }


        
    }
}
