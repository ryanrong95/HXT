using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 费用申请 原始视图
    /// </summary>
    public class ChargeAppliesOrigin : UniqueView<ChargeApply, PvFinanceReponsitory>
    {
        internal ChargeAppliesOrigin() { }

        internal ChargeAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ChargeApply> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.ChargeApplies>()
                   select new ChargeApply()
                   {
                       ID = entity.ID,
                       PayeeAccountID = entity.PayeeAccountID,
                       PayerAccountID = entity.PayerAccountID,
                       Type = (CostApplyType)entity.Type,
                       IsImmediately = entity.IsImmediately,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       CallBackUrl = entity.CallBackUrl,
                       CallBackID = entity.CallBackID,
                       SenderID = entity.SenderID,
                       Department = entity.Department,
                       ApplierID = entity.ApplierID,
                       ExcuterID = entity.ExcuterID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ApproverID = entity.ApproverID,
                       Status = (ApplyStauts)entity.Status,
                       PayerID = entity.PayerID,
                   };
        }
    }
}