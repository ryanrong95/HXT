using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// 颗粒化视图
    /// </summary>
    public class RoleView : UniqueView<Role, BvnErpReponsitory>
    {
        public RoleView()
        {

        }
        public RoleView(IGenericAdmin admin)
        {

        }

        protected override IQueryable<Role> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Roles>()
                       //where this.maps.Contains(entity.ID)
                   select new Role
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Status = (RoleStatus)entity.Status,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }
    }
}
