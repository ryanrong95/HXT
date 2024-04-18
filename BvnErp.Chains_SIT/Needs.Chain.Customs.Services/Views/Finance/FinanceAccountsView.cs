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
    public class FinanceAccountsView : UniqueView<Models.FinanceAccount, ScCustomsReponsitory>
    {
        public FinanceAccountsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceAccountsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceAccount> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var result = from account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>()
                         join vault in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>() on account.FinanceVaultID equals vault.ID into vaults
                         from vau in vaults.DefaultIfEmpty()
                         join admin in adminsView on account.AdminID equals admin.ID                        
                         where account.Status == (int)Enums.Status.Normal
                         select new Models.FinanceAccount
                         {
                             ID = account.ID,
                             FinanceVaultID = vau==null?null:vau.ID,
                             FinanceVaultName = vau == null ? null : vau.Name,
                             AccountName = account.AccountName,
                             BankAccount = account.BankAccount,
                             BankName = account.BankName,
                             BankAddress = account.BankAddress,
                             SwiftCode = account.SwiftCode,
                             CustomizedCode=account.CustomizedCode,
                             Currency = account.Currency,
                             Balance = account.Balance,
                             Admin = admin,
                             Status = (Enums.Status)account.Status,
                             Summary = account.Summary,
                             UpdateDate = account.UpdateDate,
                             CreateDate = account.CreateDate,
                             IsCash = account.IsCash,
                             AccountType = (Enums.AccountType)account.AccountType,
                             CompanyName = account.CompanyID,
                             AdminInchargeID = account.AdminInchargeID,
                             BigWinAccountID = account.BigWinAccountID,
                             Region = account.Region,
                             AccountSource = (Enums.AccountSource)account.Source
                         };
            return result;
        }
    }
}
