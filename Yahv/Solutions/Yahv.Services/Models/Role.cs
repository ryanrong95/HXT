using System.Collections.Generic;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : Underly.Erps.IRole
    {
        public Role() { }

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }

        public RoleStatus Status { get; set; }

        /// <summary>
        /// 是否是超级角色
        /// </summary>
        public bool IsSuper
        {
            get
            {
                return this.Status == RoleStatus.Super;
            }
        }

        public RoleType Type { get; set; }
        public IEnumerable<IRole> ChildRoles { get; internal set; }
    }
}
