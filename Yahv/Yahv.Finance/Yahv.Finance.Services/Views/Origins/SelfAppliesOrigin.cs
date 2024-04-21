using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 资金调拨申请 原始视图
    /// </summary>
    public class SelfAppliesOrigin : UniqueView<SelfApply, PvFinanceReponsitory>
    {
        internal SelfAppliesOrigin()
        {
        }

        internal SelfAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SelfApply> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SelfApplies>()
                   select new SelfApply()
                   {
                       Currency = (Currency)entity.Currency,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       Status = (ApplyStauts)entity.Status,
                       CreatorID = entity.CreatorID,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       PayerAccountID = entity.PayerAccountID,
                       PayeeAccountID = entity.PayeeAccountID,
                       ApproverID = entity.ApproverID,
                       ExcuterID = entity.ExcuterID,
                       ApplierID = entity.ApplierID,
                       SenderID = entity.SenderID,
                       Department = entity.Department,
                       CallBackID = entity.CallBackID,
                       CallBackUrl = entity.CallBackUrl,
                       TargetCurrency = (Currency)entity.TargetCurrency,
                       TargetERate = entity.TargetERate,
                       TargetPrice = entity.TargetPrice,
                   };
        }
    }
}