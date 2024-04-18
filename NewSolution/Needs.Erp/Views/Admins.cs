using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Models;

namespace Needs.Erp.Views
{
    public class Admins : Linq.QueryView<Models.Admin, Layer.Data.Sqls.BvnErpReponsitory>
    {
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
                   select new Admin { ID = entity.ID, RealName = entity.RealName, UserName = entity.UserName };
        }
    }
}
