using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 收付款视图
    /// </summary>
    public class ReceivePaymentView : QueryView<ReceivePaymentDto, PvFinanceReponsitory>
    {
        public ReceivePaymentView()
        {

        }

        public ReceivePaymentView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ReceivePaymentDto> GetIQueryable()
        {
            var accounts = new AccountsOrigin(this.Reponsitory);
            var flows = new FlowAccountsOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);
            var enterprises = new EnterprisesOrigin(this.Reponsitory);
            var goldStores = new GoldStoresOrigin(this.Reponsitory);

            return from entity in flows
                   join account in accounts on entity.AccountID equals account.ID
                   join admin in admins on entity.CreatorID equals admin.ID
                   join enterprise in enterprises on account.EnterpriseID equals enterprise.ID into _enterprise
                   from enterprise in _enterprise.DefaultIfEmpty()
                   join goldStore in goldStores on account.GoldStoreID equals goldStore.ID into _goldStore
                   from goldStore in _goldStore.DefaultIfEmpty()
                   select new ReceivePaymentDto()
                   {
                       Currency = entity.Currency,
                       AccountMethord = entity.AccountMethord,
                       AccountName = account.ShortName ?? account.Name,
                       CreateDate = entity.CreateDate,
                       EnterpriseName = enterprise.Name,
                       Target = entity.TargetAccountName,
                       Amount = entity.Price,
                       Balance = entity.Balance,
                       CreatorName = admin.RealName,
                       GoldStore = goldStore != null ? goldStore.Name : null,
                       AccountID = entity.AccountID,
                   };
        }
    }
}