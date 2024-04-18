using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientCompanyView : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        public ClientCompanyView()
        {
        }

        public ClientCompanyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var companyView = new CompaniesView(this.Reponsitory);
            var result = from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                         join company in companyView on client.CompanyID equals company.ID
                         select new Models.Client
                         {
                             ID = client.ID,
                             Company = company,
                             ClientType = (Enums.ClientType)client.ClientType,
                             ClientCode = client.ClientCode,
                             ClientRank = (Enums.ClientRank)client.ClientRank,                             
                             ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                             Status = (Enums.Status)client.Status,
                             CreateDate = client.CreateDate,
                             UpdateDate = client.UpdateDate,
                             Summary = client.Summary,
                             IsSpecified = client.IsSpecified,
                             ClientNature = client.ClientNature == null ? (int)ClientNature.Trade : (int)client.ClientNature,
                             ServiceType = client.ServiceType == null ? ServiceType.Unknown : (ServiceType)client.ServiceType,
                             StorageType = client.StorageType == null ? StorageType.Unknown : (StorageType)client.StorageType,
                             IsStorageValid = client.IsStorageValid,
                             IsValid = client.IsValid,
                             ChargeWH = (ChargeWHType)client.ChargeWH,
                             IsNormal = client.IsNormal,
                         };

            return result;
        }
    }
}
