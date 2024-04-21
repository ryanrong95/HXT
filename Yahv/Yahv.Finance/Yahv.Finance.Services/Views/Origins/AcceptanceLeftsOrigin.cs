using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 承兑调拨左表
    /// </summary>
    public class AcceptanceLeftsOrigin : UniqueView<AcceptanceLeft, PvFinanceReponsitory>
    {
        public AcceptanceLeftsOrigin()
        {
        }

        public AcceptanceLeftsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AcceptanceLeft> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AcceptanceLefts>()
                   select new AcceptanceLeft()
                   {
                       ID = entity.ID,
                       ApplyID = entity.ApplyID,
                       PayerAccountID = entity.PayerAccountID,
                       PayeeAccountID = entity.PayeeAccountID,
                       AccountMethord = (AccountMethord)entity.AccountMethord,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       Status = (GeneralStatus)entity.Status,
                   };
        }
    }
}