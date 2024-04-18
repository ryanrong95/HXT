using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 金库账户的视图
    /// </summary>
    public class FinanceAccountsOriginView : UniqueView<Models.FinanceAccount, ScCustomsReponsitory>
    {
        public FinanceAccountsOriginView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceAccountsOriginView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceAccount> GetIQueryable()
        {
           
            var result = from financeAccount in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>()
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
            return result;
        }
    }
}
