using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Services.Models;

namespace NtErp.Services.Views
{
    public class AdminClientsView_En : Needs.Linq.QueryView<Models.IMapsAdminClient, Layer.Data.Sqls.BvnErpReponsitory>
    {
        //string[] maps;
        IGenericAdmin admin;
        public AdminClientsView_En(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<IMapsAdminClient> GetIQueryable()
        {

            if (this.admin == null)
            {
                var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminClient_En>()
                           select new MapsAdminClient
                           {
                               AdminID = entity.AdminID,
                               ClientID = entity.ClientID
                           };
                return linq;
            }
            else
            {

                var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminClient_En>()
                           where entity.AdminID == admin.ID
                           select new MapsAdminClient
                           {
                               AdminID = entity.AdminID,
                               ClientID = entity.ClientID
                           };
                return linq;
            }
        }



    }
}
