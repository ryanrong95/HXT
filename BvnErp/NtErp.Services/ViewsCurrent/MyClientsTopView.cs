using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// Client
    /// </summary>
    public class MyClientsTopView<T> : ClientsTopView<T> where T : Layer.Linq.IReponsitory, IDisposable, new()
    {
        IGenericAdmin admin;
        public MyClientsTopView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        public MyClientsTopView(IGenericAdmin admin, T reponsitory) : base(reponsitory)
        {
            this.admin = admin;
        }

        protected override IQueryable<Models.ClientTop> GetIQueryable()
        {
            if (admin.IsSa)
            {
                return base.GetIQueryable();
            }
            return from map in Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.TopMapsAdminClient>()
                   join entity in base.GetIQueryable() on map.ClientID equals entity.ID
                   where map.AdminID == this.admin.ID
                   select entity;
        }
    }
}
