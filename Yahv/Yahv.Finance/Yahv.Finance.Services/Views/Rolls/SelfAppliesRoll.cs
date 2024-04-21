using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Finance.Services.Views.Origins;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 调拨申请 视图
    /// </summary>
    public class SelfAppliesRoll : UniqueView<SelfApply, PvFinanceReponsitory>
    {
        public SelfAppliesRoll()
        {
        }

        public SelfAppliesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SelfApply> GetIQueryable()
        {
            var accounts = new AccountsOrigin(this.Reponsitory);
            var senders = new SendersOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);
            var selfApplies = new SelfAppliesOrigin(this.Reponsitory);

            return from entity in selfApplies
                   join payerAccount in accounts on entity.PayerAccountID equals payerAccount.ID into _payerAccount
                   from payerAccount in _payerAccount.DefaultIfEmpty()
                   join payeeAccount in accounts on entity.PayeeAccountID equals payeeAccount.ID into _payeeAccount
                   from payeeAccount in _payeeAccount.DefaultIfEmpty()
                   join sender in senders on entity.SenderID equals sender.ID into _sender
                   from sender in _sender.DefaultIfEmpty()
                   join applier in admins on entity.ApplierID equals applier.ID into _applier
                   from applier in _applier.DefaultIfEmpty()
                   join approver in admins on entity.ApproverID equals approver.ID into _approver
                   from approver in _approver.DefaultIfEmpty()
                   join excuter in admins on entity.ExcuterID equals excuter.ID into _excuter
                   from excuter in _excuter.DefaultIfEmpty()
                   select new SelfApply()
                   {
                       Currency = entity.Currency,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       Status = entity.Status,
                       CreatorID = entity.CreatorID,
                       Price = entity.Price,
                       PayerAccountID = entity.PayerAccountID,
                       PayeeAccountID = entity.PayeeAccountID,
                       SenderID = entity.SenderID,
                       Summary = entity.Summary,
                       ApplierID = entity.ApplierID,
                       ExcuterID = entity.ExcuterID,
                       Department = entity.Department,
                       ApproverID = entity.ApproverID,
                       CallBackID = entity.CallBackID,
                       CallBackUrl = entity.CallBackUrl,
                       Sender = sender,
                       Applier = applier,
                       Approver = approver,
                       PayeeAccount = payeeAccount,
                       PayerAccount = payerAccount,
                       Excuter = excuter,
                       TargetPrice = entity.TargetPrice,
                       TargetCurrency = entity.TargetCurrency,
                       TargetERate = entity.TargetERate,
                   };
        }
    }
}