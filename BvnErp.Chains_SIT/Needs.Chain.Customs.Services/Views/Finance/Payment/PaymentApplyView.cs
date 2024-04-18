using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 付款申请的视图
    /// </summary>
    public class PaymentApplyView : UniqueView<Models.PaymentApply, ScCustomsReponsitory>
    {
        public PaymentApplyView()
        {
        }

        internal PaymentApplyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PaymentApply> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            var result = from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentApplies>()
                         join applier in adminView on apply.ApplierID equals applier.ID
                         join approver in adminView on apply.ApproverID equals approver.ID into approvers
                         from approver in approvers.DefaultIfEmpty()
                         orderby apply.ApplyStatus, apply.PayDate
                         where apply.ApplyStatus != (int)Enums.PaymentApplyStatus.Canceled
                         select new Models.PaymentApply
                         {
                             ID = apply.ID,
                             OrderID = apply.OrderID,
                             Applier = applier,
                             Approver = approver,
                             PayeeName = apply.PayeeName,
                             PayFeeType = (Enums.FinanceFeeType)apply.FeeType,
                             FeeDesc = apply.FeeDesc,
                             BankName = apply.BankName,
                             BankAccount = apply.BankAccount,
                             Amount = apply.Amount,
                             Currency = apply.Currency,
                             PayDate = apply.PayDate,
                             PayType = (Enums.PaymentType)apply.PayType,
                             CreateDate = apply.CreateDate,
                             UpdateDate = apply.UpdateDate,
                             ApplyStatus = (Enums.PaymentApplyStatus)apply.ApplyStatus,
                             Summary = apply.Summary,
                         };
            return result;
        }
    }
}
