using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{

    public class ClientAdminsView : UniqueView<Models.ClientAdmin, ScCustomsReponsitory>
    {
        public ClientAdminsView()
        {
        }

        internal ClientAdminsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientAdmin> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from clientAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>()
                   join admin in adminsView on clientAdmin.AdminID equals admin.ID
                   join adminwl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>() on admin.ID equals adminwl.AdminID into temp_adminwl
                   from adminwl in temp_adminwl.DefaultIfEmpty()
                   where clientAdmin.Status == (int)Enums.Status.Normal
                   select new Models.ClientAdmin
                   {
                       ID = clientAdmin.ID,
                       Admin = admin,
                       ClientID = clientAdmin.ClientID,
                       Status = (Enums.Status)clientAdmin.Status,
                       Type = (Enums.ClientAdminType)clientAdmin.Type,
                       CreateDate = clientAdmin.CreateDate,
                       Summary = clientAdmin.Summary,
                       DepartmentID = adminwl.DepartmentID
                   };
        }
    }
}
