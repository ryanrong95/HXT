using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecTaxFlowsView : UniqueView<Models.DecTaxFlow, ScCustomsReponsitory>
    {
        public DecTaxFlowsView()
        {
        }

        public DecTaxFlowsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecTaxFlow> GetIQueryable()
        {
            return from flow in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                   select new Models.DecTaxFlow
                   {
                       ID = flow.ID,
                       DecheadID = flow.DecTaxID,
                       TaxNumber = flow.TaxNumber,
                       TaxType = (Enums.DecTaxType)flow.TaxType,
                       Amount = flow.Amount,
                       PayDate = flow.PayDate,
                       BankName = flow.BankName,
                       DeductionTime = flow.DeductionTime,
                       FillinDate = flow.FillinDate,
                       Status = (Enums.DecTaxStatus)flow.Status,
                      // IsUpload=(Enums.UploadStatus)flow.IsUpload,
                       CreateDate = flow.CreateDate,
                       UpdateDate = flow.UpdateDate,
                       Summary = flow.Summary,
                   };
        }
    }
}
