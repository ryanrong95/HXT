using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 金库账户扩展方法
    /// </summary>
    public static class FinanceAccountExtends
    {
        public static Layer.Data.Sqls.ScCustoms.FinanceAccounts ToLinq(this Models.FinanceAccount entity)
        {
            return new Layer.Data.Sqls.ScCustoms.FinanceAccounts
            {
                ID = entity.ID,
                FinanceVaultID = entity.FinanceVaultID,
                AdminID = entity.Admin.ID,
                AccountName = entity.AccountName,
                BankAccount = entity.BankAccount,
                BankName = entity.BankName,
                BankAddress = entity.BankAddress,
                SwiftCode = entity.SwiftCode,
                Balance = entity.Balance,
                Currency = entity.Currency,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CustomizedCode=entity.CustomizedCode,
                Summary = entity.Summary,
                AccountType = (int)entity.AccountType,
                AdminInchargeID = entity.AdminInchargeID,
                CompanyID = entity.CompanyName,
                BigWinAccountID = entity.BigWinAccountID,
                Region = entity.Region,
                Source = (int)entity.AccountSource
            };
        }
    }
}
