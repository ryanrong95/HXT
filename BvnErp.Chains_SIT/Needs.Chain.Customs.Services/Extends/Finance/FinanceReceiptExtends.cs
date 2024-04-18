using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class FinanceReceiptExtends
    {
        public static Layer.Data.Sqls.ScCustoms.FinanceReceipts ToLinq(this Models.FinanceReceipt entity)
        {
            return new Layer.Data.Sqls.ScCustoms.FinanceReceipts
            {
                ID = entity.ID,
                SeqNo = entity.SeqNo,
                Payer = entity.Payer,
                FeeType = (int)entity.FeeType,
                ReceiptType = (int)entity.ReceiptType,
                ReceiptDate = entity.ReceiptDate,
                Currency = entity.Currency,
                Rate = entity.Rate,
                Amount = entity.Amount,
                FinanceVaultID = entity.Vault.ID,
                FinanceAccountID = entity.Account.ID,
                AdminID = entity.Admin.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                AccountProperty = (int)entity.AccountProperty
            };
        }
    }

}
