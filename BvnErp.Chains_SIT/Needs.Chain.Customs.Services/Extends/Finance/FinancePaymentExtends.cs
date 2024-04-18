using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class FinancePaymentExtends
    {
        public static Layer.Data.Sqls.ScCustoms.FinancePayments ToLinq(this Models.FinancePayment entity)
        {
            return new Layer.Data.Sqls.ScCustoms.FinancePayments
            {
                ID = entity.ID,
                SeqNo = entity.SeqNo,
                PayerID = entity.Payer.ID,
                PayeeName = entity.PayeeName,
                BankName = entity.BankName,
                BankAccount = entity.BankAccount,
                FinanceVaultID = entity.FinanceVault.ID,
                FinanceAccountID = entity.FinanceAccount.ID,
                FeeType = (int)entity.PayFeeType,
                Amount = entity.Amount,
                Currency =entity.Currency,
                ExchangeRate = entity.ExchangeRate,
                PayType = (int)entity.PayType,
                PayDate = entity.PayDate,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
