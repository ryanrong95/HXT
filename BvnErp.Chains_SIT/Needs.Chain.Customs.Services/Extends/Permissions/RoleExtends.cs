using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 角色扩展方法
    /// </summary>
    public static class RoleExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Roles ToLinq(this Models.Role entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Roles
            {
                ID = entity.ID,
                Name = entity.Name,
                SysCode = entity.SysCode,
                Summary = entity.Summary,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }
    }
}
