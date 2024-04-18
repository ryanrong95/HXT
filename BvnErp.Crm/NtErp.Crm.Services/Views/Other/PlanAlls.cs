using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class PlanAlls : UniqueView<Plan, BvCrmReponsitory>, Needs.Underly.IFkoView<Plan>
    {

        protected PlanAlls()
        {

        }

        protected override IQueryable<Plan> GetIQueryable()
        {
            return this.GetIQueryable(Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Actions>());
        }

        internal IQueryable<Plan> GetIQueryable(IQueryable<Layer.Data.Sqls.BvCrm.Actions> query = null)
        {
            ClientAlls clientview = new ClientAlls(this.Reponsitory);
            CompanyAlls companyview = new CompanyAlls(this.Reponsitory);
            AdminTopView adminview = new AdminTopView(this.Reponsitory);

            if (query == null)
            {
                query = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Actions>();
            }

            return from action in query
                   join clients in clientview on action.ClientID equals clients.ID
                   join companys in companyview on action.CompanyID equals companys.ID
                   join admin in adminview on action.AdminID equals admin.ID
                   select new Plan
                   {
                       ID = action.ID,
                       Name = action.Name,
                       Admin = admin,
                       client = clients,
                       Companys = companys,        
                       Target = (ActionTarget)action.Target,
                       Methord = (ActionMethord)action.Methord,
                       CatalogueID = action.CatalogueID,                          
                       PlanDate = action.PlanDate,
                       StartDate = action.StartDate,
                       EndDate = action.EndDate,
                       Status = (ActionStatus)action.Status,
                       CreateDate = action.CreateDate,
                       UpdateDate = action.UpdateDate,
                       Summary = action.Summary,
                       SaleID = action.SaleID,
                       SaleManagerID = action.SaleManagerID,
                   };
        }
    }
}
