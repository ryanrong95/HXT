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
    public static class FinanceAccountFlowExtends
    {
        public static Layer.Data.Sqls.ScCustoms.FinanceAccountFlows ToLinq(this Models.FinanceAccountFlow entity)
        {
            return new Layer.Data.Sqls.ScCustoms.FinanceAccountFlows
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                SeqNo = entity.SeqNo,
                SourceID = entity.SourceID,
                FinanceVaultID = entity.FinanceVault.ID,
                FinanceAccountID = entity.FinanceAccount.ID,
                Type = (int)entity.Type,
                FeeType = (int)entity.FeeType,
                PaymentType =(int)entity.PaymentType,
                Amount = entity.Amount,
                Currency = entity.Currency,
                AccountBalance = entity.AccountBalance,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
