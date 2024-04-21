using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views
{
    public class AgentEnterprisesView : Yahv.Linq.UniqueView<AgentEnterprise, PvbCrmReponsitory>
    {
        public AgentEnterprisesView()
        {

        }
        protected override IQueryable<AgentEnterprise> GetIQueryable()
        {
            return from enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                   join _clients in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Clients>() on enterprise.ID equals _clients.ID into clientsAll
                   from clients in clientsAll.DefaultIfEmpty()

                   join _suppliers in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Suppliers>()
                   on enterprise.ID equals _suppliers.ID into suppliersAll
                   from suppliers in suppliersAll.DefaultIfEmpty()
                   where clients.Status == (int)ApprovalStatus.Normal || suppliers.Status == (int)(int)ApprovalStatus.Normal
                   select new AgentEnterprise
                   {
                       ID = enterprise.ID,
                       Name = enterprise.Name,
                       Client = clients == null ? null : new apiClient
                       {
                          Grade = clients.Grade,
                          Type = (ClientType)clients.Nature
                       },
                       Supplier=suppliers==null?null:new apiSupplier {
                           Grade = suppliers.Grade,
                           IsFactory = suppliers.IsFactory,
                       }
                   };
        }
    }
}
