using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class RefundApprove : RefundHandler
    {
        public override void HandleRequest(RefundApply apply)
        {
            if (apply.ApplyStatus == Enums.RefundApplyStatus.Approved)
            {
                apply.Approve(Enums.RefundApplyStatus.Approved);

                PaymentNotice paymentNotice = new PaymentNotice();
                paymentNotice.Admin = apply.Applicant; //申请人
                paymentNotice.Payer = apply.Payer;  //财务的ID
                paymentNotice.PayFeeType = Needs.Ccs.Services.Enums.FinanceFeeType.PaySaleAgentBack;
                paymentNotice.FeeDesc = "退款";
                paymentNotice.PayeeName = apply.PayeeAccount.AccountName;
                paymentNotice.BankName = apply.PayeeAccount.BankName;
                paymentNotice.BankAccount = apply.PayeeAccount.BankAccount;
                paymentNotice.Amount = apply.Amount;
                paymentNotice.Currency = apply.Currency;
                paymentNotice.PayDate = DateTime.Now;
                paymentNotice.PayType = Needs.Ccs.Services.Enums.PaymentType.TransferAccount;
                paymentNotice.Status = Needs.Ccs.Services.Enums.PaymentNoticeStatus.UnPay;
                paymentNotice.CreateDate = DateTime.Now;
                paymentNotice.UpdateDate = DateTime.Now;
                paymentNotice.RefundApplyID = apply.ID;
                paymentNotice.ExchangeRate = apply.ExchangeRate;

                paymentNotice.Enter();

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                   var ReceiptNotice =  reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>().Where(t => t.ID == apply.FinanceReceiptID).FirstOrDefault();
                   decimal availableAmount = ReceiptNotice.AvailableAmount;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(
                        new
                        {
                            AvailableAmount = availableAmount-apply.Amount,
                            UpdateDate = DateTime.Now
                        }, item => item.ID == apply.FinanceReceiptID);
                }
            }
            else 
            {
                if (this.successor != null)
                    successor.HandleRequest(apply);
            }
        }
    }
}
