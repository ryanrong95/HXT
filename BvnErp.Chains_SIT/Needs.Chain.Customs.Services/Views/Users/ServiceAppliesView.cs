using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ServiceAppliesView : UniqueView<Models.ServiceApplies, ScCustomsReponsitory>
    {
        public ServiceAppliesView()
        {
        }

        internal ServiceAppliesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ServiceApplies> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from service in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ServiceApplies>()
                   join admin in adminsView on service.AdminID equals admin.ID into serviceadmin
                   from sa in serviceadmin.DefaultIfEmpty()
                   orderby service.CreateDate descending
                   select new Models.ServiceApplies
                   {
                       ID = service.ID,
                       Email = service.Email,
                       CompanyName = service.CompanyName,
                       Address = service.Address,
                       Contact = service.Contact,
                       Mobile = service.Mobile,
                       Tel = service.Tel,
                       Admin = sa,
                       Status = (Enums.HandleStatus)service.Status,
                       CreateDate = service.CreateDate,
                       UpdateDate = service.UpdateDate,
                       Summary = service.Summary
                   };
        }
    }
}
