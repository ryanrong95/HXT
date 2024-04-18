using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class AdminRoleExtends
    {
        public static Layer.Data.Sqls.ScCustoms.AdminRoles ToLinq(this Models.AdminRoles entity)
        {
            return new Layer.Data.Sqls.ScCustoms.AdminRoles
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                RoleID = entity.Role.ID,
                CreateDate = entity.CreateDate,
            };
        }
    }
}
