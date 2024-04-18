using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class FundTransferApplyExtends
    {
        public static Layer.Data.Sqls.ScCustoms.FundTransferApplies ToLinq(this Models.FundTransferApplies entity)
        {
            return new Layer.Data.Sqls.ScCustoms.FundTransferApplies
            {
               ID = entity.ID,
               FromSeqNo = entity.FromSeqNo,
               OutAccountID = entity.OutAccount.ID,
               OutSeqNo = entity.OutSeqNo,
               OutAmount = entity.OutAmount,
               OutCurrency = entity.OutCurrency,
               InAccountID = entity.InAccount.ID,
               InSeqNo = entity.InSeqNo,
               InAmount = entity.InAmount,
               InCurrency = entity.InCurrency,
               Rate = entity.Rate,
               DiscountInterest = entity.DiscountInterest,
               Poundage = entity.Poundage,
               PoundageSeqNo = entity.PoundageSeqNo,
               PaymentType = (int)entity.PaymentType,
               PaymentDate = entity.PaymentDate,
               FeeType = (int)entity.FeeType,
               OrderID = entity.OrderID,
               ApplyStatus = (int)entity.ApplyStatus,
               AdminID = entity.Admin.ID,
               Status = (int)entity.Status,
               CreateDate = entity.CreateDate,
               UpdateDate = entity.UpdateDate,
               Summary = entity.Summary,
               QRCodeFee = entity.QRCodeFee
            };
        }
    }
}
