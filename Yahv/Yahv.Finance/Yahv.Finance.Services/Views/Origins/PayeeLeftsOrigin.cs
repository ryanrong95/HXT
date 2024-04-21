using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    public class PayeeLeftsOrigin : UniqueView<PayeeLeft, PvFinanceReponsitory>
    {
        internal PayeeLeftsOrigin() { }

        internal PayeeLeftsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PayeeLeft> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayeeLefts>()
                   select new PayeeLeft()
                   {
                       ID = entity.ID,
                       AccountCatalogID = entity.AccountCatalogID,
                       AccountID = entity.AccountID,
                       PayerName = entity.PayerName,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       Currency1 = (Currency)entity.Currency1,
                       ERate1 = entity.ERate1,
                       Price1 = entity.Price1,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       FlowID = entity.FlowID,
                       Status = (GeneralStatus)entity.Status,
                       Summary = entity.Summary,
                       PayerNature = (NatureType)entity.PayerNature,
                   };
        }
    }
}
