using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 资金调拨 应调视图
    /// </summary>
    public class SelfLeftsOrigin : UniqueView<SelfLeft, PvFinanceReponsitory>
    {
        internal SelfLeftsOrigin()
        {
        }

        internal SelfLeftsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SelfLeft> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SelfLefts>()
                   select new SelfLeft()
                   {
                       Currency = (Currency)entity.Currency,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       Status = (GeneralStatus)entity.Status,
                       CreatorID = entity.CreatorID,
                       Price = entity.Price,
                       Price1 = entity.Price1,
                       PayerAccountID = entity.PayerAccountID,
                       PayeeAccountID = entity.PayeeAccountID,
                       ApplyID = entity.ApplyID,
                       Currency1 = (Currency)entity.Currency1,
                       ERate1 = entity.ERate1,
                       AccountCatalogID = entity.AccountCatalogID,
                       AccountMethord = (AccountMethord)entity.AccountMethord,
                   };
        }
    }
}