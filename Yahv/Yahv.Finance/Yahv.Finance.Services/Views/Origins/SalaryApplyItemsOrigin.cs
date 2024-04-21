using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 工资申请项 原始视图
    /// </summary>
    public class SalaryApplyItemsOrigin : UniqueView<SalaryApplyItem, PvFinanceReponsitory>
    {
        internal SalaryApplyItemsOrigin()
        {
        }

        internal SalaryApplyItemsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SalaryApplyItem> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SalaryApplyItems>()
                   select new SalaryApplyItem()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       Status = (ApplyItemStauts)entity.Status,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       PayeeAccountID = entity.PayeeAccountID,
                       ApplyID = entity.ApplyID,
                       ModifyDate = entity.ModifyDate,
                       PayerAccountID = entity.PayerAccountID,
                       AccountCatalogID = entity.AccountCatalogID,
                       CallBackID = entity.CallBackID,
                       FlowID = entity.FlowID,
                       CallBackUrl = entity.CallBackUrl
                   };
        }
    }
}