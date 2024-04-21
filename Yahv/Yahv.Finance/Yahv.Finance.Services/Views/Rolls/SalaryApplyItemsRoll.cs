using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 工资申请项 视图
    /// </summary>
    public class SalaryApplyItemsRoll : UniqueView<SalaryApplyItem, PvFinanceReponsitory>
    {
        public SalaryApplyItemsRoll()
        {
        }

        public SalaryApplyItemsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SalaryApplyItem> GetIQueryable()
        {
            var applyItems = new SalaryApplyItemsOrigin(this.Reponsitory);
            var flows = new FlowAccountsOrigin(this.Reponsitory);
            var accounts = new AccountsOrigin(this.Reponsitory);
            var persons = new PersonsOrigin(this.Reponsitory);
            var enterprises = new EnterprisesOrigin(this.Reponsitory);

            return from entity in applyItems
                   join flow in flows on entity.FlowID equals flow.ID into _flow
                   from flow in _flow.DefaultIfEmpty()

                   join payeeAccount in accounts on entity.PayeeAccountID equals payeeAccount.ID into _payeeAccounts
                   from payeeAccount in _payeeAccounts.DefaultIfEmpty()

                   join payerAccount in accounts on entity.PayerAccountID equals payerAccount.ID into _payerAccount
                   from payerAccount in _payerAccount.DefaultIfEmpty()

                   join person in persons on payeeAccount.PersonID equals person.ID into _person
                   from person in _person.DefaultIfEmpty()

                   join enterprise in enterprises on payeeAccount.EnterpriseID equals enterprise.ID into _enterprise
                   from enterprise in _enterprise.DefaultIfEmpty()

                   select new SalaryApplyItem()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       Status = entity.Status,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       PayeeAccountID = entity.PayeeAccountID,
                       ApplyID = entity.ApplyID,
                       PayerAccountID = entity.PayerAccountID,
                       ModifyDate = entity.ModifyDate,
                       AccountCatalogID = entity.AccountCatalogID,
                       CallBackID = entity.CallBackID,
                       CallBackUrl = entity.CallBackUrl,
                       FlowID = entity.FlowID,

                       PaymentDate = flow.PaymentDate,
                       PayeeName = flow.TargetAccountName,
                       PayeeCode = flow.TargetAccountCode,
                       FormCode = flow.FormCode,
                       PayerCode = payerAccount.Code,
                       PayeeIDCard = person != null ? person.IDCard : "",
                       PayeeCompany = enterprise != null ? enterprise.Name : "",
                   };
        }
    }
}