using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class FinanceAccountsOrigin : UniqueView<Models.FinanceAccount, ScCustomsReponsitory>
    {
        internal FinanceAccountsOrigin()
        {
        }

        internal FinanceAccountsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceAccount> GetIQueryable()
        {
            return from financeAccount in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>()
                   select new FinanceAccount
                   {
                       ID = financeAccount.ID,
                       FinanceVaultID = financeAccount.FinanceVaultID,
                       AccountName = financeAccount.AccountName,
                       BankAccount = financeAccount.BankAccount,
                       BankName = financeAccount.BankName,
                       BankAddress = financeAccount.BankAddress,
                       SwiftCode = financeAccount.SwiftCode,
                       Currency = financeAccount.Currency,
                       Balance = financeAccount.Balance,
                       AdminID = financeAccount.AdminID,
                       Status = (Enums.Status)financeAccount.Status,
                       CreateDate = financeAccount.CreateDate,
                       UpdateDate = financeAccount.UpdateDate,
                       Summary = financeAccount.Summary,
                       CustomizedCode = financeAccount.CustomizedCode,
                   };
        }
    }
}
