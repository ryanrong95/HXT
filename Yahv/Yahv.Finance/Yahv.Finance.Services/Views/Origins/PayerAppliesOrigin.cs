using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 货款申请 原始视图
    /// </summary>
    public class PayerAppliesOrigin : UniqueView<PayerApply, PvFinanceReponsitory>
    {
        public PayerAppliesOrigin()
        {

        }

        public PayerAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<PayerApply> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerApplies>()
                   select new PayerApply()
                   {
                       ID = entity.ID,
                       Status = (ApplyStauts)entity.Status,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       ApplierID = entity.ApplierID,
                       IsPaid = entity.IsPaid,
                       ExcuterID = entity.ExcuterID,
                       SenderID = entity.SenderID,
                       Summary = entity.Summary,
                       Department = entity.Department,
                       CallBackID = entity.CallBackID,
                       CallBackUrl = entity.CallBackUrl,
                       ApproverID = entity.ApproverID,
                       PayeeAccountID = entity.PayeeAccountID,
                       PayerAccountID = entity.PayerAccountID,
                       PayerID = entity.PayerID,
                   };
        }
    }
}