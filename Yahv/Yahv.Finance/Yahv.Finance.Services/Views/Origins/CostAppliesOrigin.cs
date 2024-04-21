using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    public class CostAppliesOrigin : UniqueView<CostApply, PvFinanceReponsitory>
    {
        internal CostAppliesOrigin() { }

        internal CostAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<CostApply> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.CostApplies>()
                   select new CostApply()
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
                       CostPurpose = (FundPurpose)entity.CostPurpose,
                       PayerID = entity.PayerID,
                   };
        }
    }
}
