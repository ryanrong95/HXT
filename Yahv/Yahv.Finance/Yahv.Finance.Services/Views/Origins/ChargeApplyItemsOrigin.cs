using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 费用申请项 原始视图
    /// </summary>
    public class ChargeApplyItemsOrigin : UniqueView<ChargeApplyItem, PvFinanceReponsitory>
    {
        internal ChargeApplyItemsOrigin() { }

        internal ChargeApplyItemsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ChargeApplyItem> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.ChargeApplyItems>()
                   select new ChargeApplyItem()
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