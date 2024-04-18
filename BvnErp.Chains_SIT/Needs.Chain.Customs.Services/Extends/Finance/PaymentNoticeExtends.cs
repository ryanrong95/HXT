using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付款申请扩展方法
    /// </summary>
    public static class PaymentNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PaymentNotices ToLinq(this Models.PaymentNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PaymentNotices
            {
                ID = entity.ID,
                SeqNo = entity.SeqNo,
                AdminID = entity.Admin?.ID,
                PayerID = entity.Payer?.ID,
                PaymentApplyID = entity.PaymentApply?.ID,
                PayExchangeApplyID = entity.PayExchangeApply?.ID,
                FinanceVaultID = entity.FinanceVault?.ID,
                FinanceAccountID = entity.FinanceAccount?.ID,
                FeeType = (int)entity.PayFeeType,
                FeeDesc = entity.FeeDesc,
                PayeeName = entity.PayeeName,
                BankName = entity.BankName,
                BankAccount = entity.BankAccount,
                Amount = entity.Amount,
                Currency = entity.Currency,
                ExchangeRate = entity.ExchangeRate,
                PayDate = entity.PayDate,
                PayType = (int)entity.PayType,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                CostApplyID = entity.CostApplyID,
                RefundApplyID = entity.RefundApplyID,
            };
        }
    }
}
