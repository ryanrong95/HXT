using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 认领视图
    /// </summary>
    public class AccountWorksView : QueryView<AccountWorkDto, PvFinanceReponsitory>
    {
        public AccountWorksView()
        {
        }

        public AccountWorksView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AccountWorkDto> GetIQueryable()
        {
            var payeeLefts = new PayeeLeftsOrigin(this.Reponsitory);
            var accountWorkds = new AccountWorksOrigin(this.Reponsitory);
            var accountCatalogs = new AccountCatalogsOrigin(this.Reponsitory);
            var accounts = new AccountsOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);
            var flowAccountsOrigin = new FlowAccountsOrigin(this.Reponsitory);
            var enterprises = new EnterprisesOrigin(this.Reponsitory);

            return from entity in accountWorkds
                   join payeeLeft in payeeLefts on entity.PayeeLeftID equals payeeLeft.ID
                   join accountCatalog in accountCatalogs on payeeLeft.AccountCatalogID equals accountCatalog.ID
                   join account in accounts on payeeLeft.AccountID equals account.ID
                   join admin in admins on entity.ClaimantID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   join flows in flowAccountsOrigin on payeeLeft.FlowID equals flows.ID
                   join ep in enterprises on account.EnterpriseID equals ep.ID into _ep
                   from ep in _ep.DefaultIfEmpty()
                   select new AccountWorkDto()
                   {
                       AccountCatalogName = accountCatalog.Name,
                       AccountCode = account.Code,
                       BankName = account.BankName,
                       ClaimantName = admin == null ? "" : admin.RealName,
                       Company = entity.Company,
                       Conduct = entity.Conduct,
                       CreateDate = entity.CreateDate,
                       Currency = account.Currency,
                       FormCode = flows.FormCode,
                       ID = entity.ID,
                       Price = payeeLeft.Price,
                       ModifyDate = entity.ModifyDate,
                       PayeeLeftID = entity.PayeeLeftID,
                       PayerName = payeeLeft.PayerName,
                       AccountShortName = account.ShortName ?? account.Name,
                       AccountEnterprise = ep == null ? null : ep.Name,
                       ReceiptDate = flows.PaymentDate.Value,
                   };
        }
    }
}