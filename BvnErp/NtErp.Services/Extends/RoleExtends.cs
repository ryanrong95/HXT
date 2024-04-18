using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Extends
{
    static public class RoleExtends
    {
        static public Layer.Data.Sqls.BvnErp.Roles ToLinq(this Role entity)
        {
            return new Layer.Data.Sqls.BvnErp.Roles
            {
                ID = entity.ID,
                Name = entity.Name,
                Status =(int)entity.Status,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate
            };
        }
    }
}
