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
    public static class PaymentApplyExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PaymentApplies ToLinq(this Models.PaymentApply entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PaymentApplies
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                ApplierID = entity.Applier.ID,
                ApproverID = entity.Approver?.ID,
                FeeType = (int)entity.PayFeeType,
                FeeDesc = entity.FeeDesc,
                PayeeName = entity.PayeeName,
                BankName = entity.BankName,
                BankAccount = entity.BankAccount,
                Amount = entity.Amount,
                Currency = entity.Currency,
                PayDate = entity.PayDate,
                PayType = (int)entity.PayType,
                ApplyStatus = (int)entity.ApplyStatus,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }

    public static class PaymentApplyLogExtends
    {
        /// <summary>
        /// 写入订单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.PaymentApply entity, Admin Operator, string summary)
        {
            PaymentApplyLog log = new PaymentApplyLog();
            log.PaymentApplyID = entity.ID;
            log.Admin = Operator;
            log.PaymentApplyStatus = entity.ApplyStatus;
            log.Summary = summary;
            log.Enter();
        }
    }
}
