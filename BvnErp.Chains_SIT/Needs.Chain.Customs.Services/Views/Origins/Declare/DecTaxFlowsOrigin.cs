using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class DecTaxFlowsOrigin : UniqueView<Models.DecTaxFlow, ScCustomsReponsitory>
    {
        public DecTaxFlowsOrigin()
        {
        }

        public DecTaxFlowsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecTaxFlow> GetIQueryable()
        {
            return from decTaxFlow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                   select new Models.DecTaxFlow
                   {
                       ID = decTaxFlow.ID,
                       DecTaxID = decTaxFlow.DecTaxID,
                       BankName = decTaxFlow.BankName,
                       TaxNumber = decTaxFlow.TaxNumber,
                       TaxType = (Enums.DecTaxType)decTaxFlow.TaxType,
                       PayDate = decTaxFlow.PayDate,
                       DeductionTime = decTaxFlow.DeductionTime,
                       Amount = decTaxFlow.Amount,
                       Status = (Enums.DecTaxStatus)decTaxFlow.Status,
                       CreateDate = decTaxFlow.CreateDate,
                       UpdateDate = decTaxFlow.UpdateDate,
                       Summary = decTaxFlow.Summary,
                   };
        }
    }
}
