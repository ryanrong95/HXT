using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views
{
    /// <summary>
    /// 货款申请对账单
    /// </summary>
    public class PayerStatementsView : QueryView<PayerStatement, PvFinanceReponsitory>
    {
        public PayerStatementsView()
        {
        }

        public PayerStatementsView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayerStatement> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerStatementsView>()
                   select new PayerStatement()
                   {
                       Currency = (Currency)entity.Currency,
                       Status = (GeneralStatus)entity.Status,
                       CreatorID = entity.CreatorID,
                       PayeeAccountID = entity.PayeeAccountID,
                       ApplyID = entity.ApplyID,
                       PayerAccountID = entity.PayerAccountID,
                       AccountCatalogID = entity.AccountCatalogID,
                       LeftDate = entity.LeftDate,
                       LeftID = entity.LeftID,
                       LeftPrice = entity.LeftPrice,
                       RightDate = entity.RightDate,
                       RightPrice = entity.RightPrice
                   };
        }
    }
}