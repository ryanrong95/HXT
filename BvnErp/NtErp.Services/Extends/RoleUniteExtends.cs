using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Extends
{
    static public class RoleUniteExtends
    {
        static public Layer.Data.Sqls.BvnErp.RoleUnites ToLinq(this IRoleUnite entity)
        {
            return new Layer.Data.Sqls.BvnErp.RoleUnites
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                Menu = entity.Menu,
                Name = entity.Name,
                Title = entity.Title,
                Url = entity.Url,
                CreateDate = entity.CreateDate
            };
        }
    }
}
