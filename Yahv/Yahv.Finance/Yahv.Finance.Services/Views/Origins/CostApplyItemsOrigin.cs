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
    public class CostApplyItemsOrigin : UniqueView<CostApplyItem, PvFinanceReponsitory>
    {
        internal CostApplyItemsOrigin() { }

        internal CostApplyItemsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<CostApplyItem> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.CostApplyItems>()
                   select new CostApplyItem()
                   {
                       ID = entity.ID,
                       ApplyID = entity.ApplyID,
                       IsPaid = entity.IsPaid,
                       ExpectedTime = entity.ExpectedTime,
                       AccountCatalogID = entity.AccountCatalogID,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       FlowID = entity.FlowID,
                       CallBackUrl = entity.CallBackUrl,
                       CallBackID = entity.CallBackID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (ApplyItemStauts)entity.Status,
                   };
        }
    }
}
