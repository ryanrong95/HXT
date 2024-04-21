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
    public class FlowAccountsOrigin : UniqueView<FlowAccount, PvFinanceReponsitory>
    {
        internal FlowAccountsOrigin() { }

        internal FlowAccountsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.FlowAccounts>()
                   select new FlowAccount()
                   {
                       ID = entity.ID,
                       AccountMethord = (AccountMethord)entity.AccountMethord,
                       AccountID = entity.AccountID,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       Balance = entity.Balance,
                       PaymentDate = entity.PaymentDate,
                       FormCode = entity.FormCode,
                       Currency1 = (Currency)entity.Currency1,
                       ERate1 = entity.ERate1,
                       Price1 = entity.Price1,
                       Balance1 = entity.Balance1,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       TargetAccountName = entity.TargetAccountName,
                       TargetAccountCode = entity.TargetAccountCode,
                       PaymentMethord = (PaymentMethord)entity.PaymentMethord,
                       TargetRate = entity.TargetRate,
                       Type = entity.Type == null ? FlowAccountType.BankStatement : (FlowAccountType)entity.Type,
                       MoneyOrderID = entity.MoneyOrderID,
                   };
        }
    }
}
