using Yahv.Models;
using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
using Yahv.Underly;

namespace Yahv.Views
{
    class AdminsAlls : UniqueView<ErpAdmin, PvbErmReponsitory>
    {
        protected internal AdminsAlls()
        {
        }

        internal AdminsAlls(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<ErpAdmin> GetIQueryable()
        {
            //var composeView = from c in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
            //                  join r in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>() on c.ChildID equals r.ID
            //                  select new
            //                  {
            //                      FatherID = c.RoleID,
            //                      r.ID,
            //                      r.Name,
            //                      r.Status,
            //                      r.Type,
            //                  };

            return from admin in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>()
                   join _role in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>() on admin.RoleID equals _role.ID into roles
                   from role in roles.DefaultIfEmpty()
                   //join compose in composeView on role.ID equals compose.FatherID into composes
                   select new ErpAdmin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       Role = new Role
                       {
                           ID = role.ID,
                           Name = role.Name,
                           Status = (RoleStatus)role.Status,
                           Type = (RoleType)role.Type,
                       },
                       Status = (AdminStatus)admin.Status,
                       StaffID = admin.StaffID,
                   };
        }
    }
}

