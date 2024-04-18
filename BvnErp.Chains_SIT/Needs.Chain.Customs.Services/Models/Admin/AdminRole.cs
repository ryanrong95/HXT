using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 管理员角色
    /// </summary>
    public class AdminRole
    {
        /// <summary>
        /// AdminID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// RoleID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        #region 扩展属性

        /// <summary>
        /// 归类使用
        /// </summary>
        public Enums.DeclarantRole DeclarantRole
        {
            get
            {
                if (this.RoleName == ClassifyConfig.juniorDeclarant)
                    return Enums.DeclarantRole.JuniorDeclarant;
                else
                    return Enums.DeclarantRole.SeniorDeclarant;
            }
        }

        #endregion
    }
}
