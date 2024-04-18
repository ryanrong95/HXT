using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Extends
{
    static public class MenuExtends
    {
        static public Layer.Data.Sqls.BvnErp.Menus ToLinq(this Menu entity)
        {
            return new Layer.Data.Sqls.BvnErp.Menus
            {
                ID = entity.ID,
                Name = entity.Name,
                Icon = entity.Icon,
                FatherID = entity.FatherID,
                Url = entity.Url,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                OrderIndex = entity.OrderIndex
            };
        }
    }
}
