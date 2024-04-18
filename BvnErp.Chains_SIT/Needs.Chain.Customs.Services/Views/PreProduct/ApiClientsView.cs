using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ApiClientsView : UniqueView<Models.ApiClient, ScCustomsReponsitory>
    {
        public ApiClientsView()
        {
        }

        internal ApiClientsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ApiClient> GetIQueryable()
        {           
            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiClient>()
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on para.ClientID equals client.ID
                   select new Models.ApiClient
                   {
                       ID = para.ID,
                       ClientId = para.ClientID,
                       ClientCode = client.ClientCode,
                       ClientSupplierID = para.ClientSupplierID,
                       AccountName = para.AccountName,
                       Password = para.Password,
                       CompanyCode = para.CompanyCode,
                       Status = (Status)para.Status,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate,
                       Summary = para.Summary,
                   };
        }
    }
}
