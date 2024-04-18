using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Rolls
{
    internal class ClientsRoll : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        internal ClientsRoll()
        {
        }

        internal ClientsRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var clientsView = new Origins.ClientsOrigin(this.Reponsitory);
            var companiesView = new CompaniesRoll(this.Reponsitory);
            return from client in clientsView
                   join company in companiesView on client.CompanyID equals company.ID
                   select new Models.Client
                   {
                       ID = client.ID,
                       Company = company,
                       ClientType = client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = client.ClientRank,
                       ClientStatus = client.ClientStatus,
                       Status = client.Status,
                       AdminID = client.AdminID,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       Summary = client.Summary,
                   };
        }
    }
}
