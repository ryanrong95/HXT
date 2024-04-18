using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class AdminsView : QueryView<Admin, PvWmsRepository>
    {
        protected override IQueryable<Admin> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>()
                   select new Admin
                   {
                       ID = entity.ID,
                       UserName = entity.UserName,
                       StaffID = entity.StaffID,
                       RealName = entity.RealName,
                       Status = (AdminStatus)entity.Status,
                       RoleID = entity.RoleID,
                       RoleName = entity.RoleName,
                       SelCode = entity.SelCode

                   };
        }
    }
}
