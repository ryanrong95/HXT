using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class ClientsOrigin : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        public ClientsOrigin()
        {
        }

        public ClientsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Client> GetIQueryable()
        {
            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   select new Models.Client
                   {
                       ID = client.ID,
                       CompanyID = client.CompanyID,
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Enums.ClientRank)client.ClientRank,
                       ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                       Status = (Enums.Status)client.Status,
                       AdminID = client.AdminID,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       Summary = client.Summary,
                   };
        }
    }
}
