using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 承兑调拨申请 原始视图
    /// </summary>
    public class AcceptanceAppliesOrigin : UniqueView<AcceptanceApply, PvFinanceReponsitory>
    {
        public AcceptanceAppliesOrigin()
        {
        }

        public AcceptanceAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AcceptanceApply> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AcceptanceApplies>()
                   select new AcceptanceApply()
                   {
                       ID = entity.ID,
                       Type = (AcceptanceType)entity.Type,
                       PayerAccountID = entity.PayerAccountID,
                       PayeeAccountID = entity.PayeeAccountID,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       SenderID = entity.SenderID,
                       ApplierID = entity.ApplierID,
                       ExcuterID = entity.ExcuterID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ApproverID = entity.ApproverID,
                       Status = (ApplyStauts)entity.Status,
                       MoneyOrderID = entity.MoneyOrderID,
                   };
        }
    }
}