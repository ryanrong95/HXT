using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Plats.Services.Models
{
    /// <summary>
    /// Erp 角色
    /// </summary>
    public class RoleRoll : Yahv.Underly.Erps.IRole
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; internal set; }

        public RoleStatus Status { get; internal set; }

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

        public RoleType Type { get; internal set; }
        public IEnumerable<IRole> ChildRoles { get; internal set; }

        internal RoleRoll() { }
    }
}
