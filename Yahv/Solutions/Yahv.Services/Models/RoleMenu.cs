using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 角色菜单
    /// </summary>
    public class RoleMenu
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public string MenuID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        public string RightUrl { get; set; }
        public string FirstUrl { get; set; }
        public string LogoUrl { get; set; }
        public int? OrderIndex { get; set; }
    }
}
