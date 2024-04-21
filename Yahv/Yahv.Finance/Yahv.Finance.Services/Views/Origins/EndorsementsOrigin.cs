using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 背书转让 原始视图
    /// </summary>
    public class EndorsementsOrigin : UniqueView<Endorsement, PvFinanceReponsitory>
    {
        public EndorsementsOrigin()
        {
        }

        public EndorsementsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Endorsement> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Endorsements>()
                   select new Endorsement()
                   {
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       EndorseDate = entity.EndorseDate,
                       ID = entity.ID,
                       PayerAccountID = entity.PayerAccountID,
                       Summary = entity.Summary,
                       IsTransfer = entity.IsTransfer,
                       MoneyOrderID = entity.MoneyOrderID,
                       PayeeAccountID = entity.PayeeAccountID,
                   };
        }
    }
}