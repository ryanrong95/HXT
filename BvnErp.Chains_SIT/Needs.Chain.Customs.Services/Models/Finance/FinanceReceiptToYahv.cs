using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FinanceReceiptToYahv
    {
        private FinanceReceipt FinanceReceipt { get; set; }

        public FinanceReceiptToYahv(FinanceReceipt financeReceipt)
        {
            this.FinanceReceipt = financeReceipt;
        }

        public void Execute()
        {
            try
            {
                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == this.FinanceReceipt.Admin.ID).FirstOrDefault();

                Yahv.Payments.PaymentManager.Erp(admin.ErmAdminID)[this.FinanceReceipt.Payer, PurchaserContext.Current.CompanyName].Digital.AdvanceFromCustomers(
                                                Yahv.Underly.Currency.CNY,
                                                this.FinanceReceipt.Amount,
                                                this.FinanceReceipt.Account.BankName,
                                                this.FinanceReceipt.Account.BankAccount,
                                                this.FinanceReceipt.SeqNo,
                                                this.FinanceReceipt.ReceiptDate);
            }
            catch (Exception ex)
            {
                ex.CcsLog("导入账户流水传Yahv(FinanceReceiptToYahv)");
            }
        }

    }
}
