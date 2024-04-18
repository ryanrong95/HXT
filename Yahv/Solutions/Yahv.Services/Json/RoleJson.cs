using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Json
{
    public class RoleJson
    {
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

    }
}
