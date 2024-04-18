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
    public class ReceivablesOriginView : UniqueView<Models.Receivables, ScCustomsReponsitory>
    {
        public ReceivablesOriginView()
        {

        }

        internal ReceivablesOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Receivables> GetIQueryable()
        {
            var result = from pricing in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Receivables>()
                         where pricing.Status == (int)Status.Normal
                         select new Models.Receivables
                         {
                             ID = pricing.ID,
                             FinanceReceiptID = pricing.FinanceReceiptID,
                             ClientID = pricing.ClientID,
                             Amount = pricing.Amount,
                             Currency = pricing.Currency,
                             CNYAmount = pricing.CNYAmount,
                             MatchStatus = (MatchStatusEnums)pricing.MatchStatus,
                             Status = (Status)pricing.Status,
                             CreateDate = pricing.CreateDate,
                             UpdateDate = pricing.UpdateDate,
                             Summary = pricing.Summary
                         };

            return result;
        }
    }
}
