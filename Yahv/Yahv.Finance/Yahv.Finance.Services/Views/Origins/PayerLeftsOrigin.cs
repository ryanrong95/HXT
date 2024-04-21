using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 应付（货款）
    /// </summary>
    public class PayerLeftsOrigin : UniqueView<PayerLeft, PvFinanceReponsitory>
    {
        internal PayerLeftsOrigin()
        {

        }

        internal PayerLeftsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PayerLeft> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerLefts>()
                   select new PayerLeft()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       Status = (GeneralStatus)entity.Status,
                       Currency = (Currency)entity.Currency,
                       CreatorID = entity.CreatorID,
                       Price = entity.Price,
                       Price1 = entity.Price1,
                       Currency1 = (Currency)entity.Currency1,
                       ERate1 = entity.ERate1,
                       PayeeAccountID = entity.PayeeAccountID,
                       ApplyID = entity.ApplyID,
                       PayerAccountID = entity.PayerAccountID,
                       AccountCatalogID = entity.AccountCatalogID,
                       PayerID = entity.PayerID,
                   };
        }
    }
}