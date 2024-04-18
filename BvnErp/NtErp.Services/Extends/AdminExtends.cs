using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Services.Models;

namespace NtErp.Services.Extends
{
    public static class AdminExtends
    {
        public static Layer.Data.Sqls.BvnErp.Admins ToLinq(this Admin entity)
        {
            return new Layer.Data.Sqls.BvnErp.Admins
            {
                ID = entity.ID,
                UserName = entity.UserName,
                RealName = entity.RealName,
                Password = entity.Password,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                LoginDate = entity.LoginDate,
                Summary = entity.Summary,
                Status = (int)entity.Status
            };
        }

       
    }
}
