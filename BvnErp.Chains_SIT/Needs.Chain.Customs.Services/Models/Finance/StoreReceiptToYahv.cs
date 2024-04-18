using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;

namespace Needs.Ccs.Services.Models
{
    public class StoreReceiptToYahv
    {
        /// <summary>
        /// 小订单ID
        /// </summary>
        private string MainOrderID { get; set; }

        /// <summary>
        /// Admin
        /// </summary>
        private Admin Admin { get; set; }

        /// <summary>
        /// FinanceReceiptID
        /// </summary>
        private string FinanceReceiptID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        private string ClienName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string ApplicationID { get; set; }

        /// <summary>
        /// 本次提交的所有费用
        /// </summary>
        private StoreReceiptToYahvAmountModel[] Fees { get; set; }

        public StoreReceiptToYahv(string mainOrderID, Admin admin, string financeReceiptID, string clienName, string applicationID, StoreReceiptToYahvAmountModel[] fees)
        {
            this.MainOrderID = mainOrderID;
            this.Admin = admin;
            this.FinanceReceiptID = financeReceiptID;
            this.ClienName = clienName;
            this.ApplicationID = applicationID;
            this.Fees = fees;
        }

        public void Execute()
        {
            try
            {
                List<Yahv.Payments.Models.Fee> fees = new List<Yahv.Payments.Models.Fee>();
                if (this.Fees == null || !this.Fees.Any())
                {
                    return;
                }

                foreach (var item in this.Fees)
                {
                    fees.Add(new Yahv.Payments.Models.Fee()
                    {
                        LeftID = item.ReceivableID,
                        RightPrice = item.Amount,
                    });
                }

                //查出 流水号, 银行, 银行卡号
                var backData = new Views.ReceiptToYahvView().GetData(this.FinanceReceiptID);

                Yahv.Payments.Models.Rolls.VoucherInput entity = new Yahv.Payments.Models.Rolls.VoucherInput()
                {
                    CreateDate = DateTime.Now,
                    Type = Yahv.Underly.VoucherType.Receipt,
                    Payer = this.ClienName,
                    Payee = PurchaserContext.Current.CompanyName,
                    Bank = backData.BankName,
                    Account = backData.BankAccount,
                    Currency = Yahv.Underly.Currency.CNY,
                    //Beneficiary = beneficiary.ID,
                    CreatorID = this.Admin.ErmAdminID,
                    FormCode = backData.SeqNo,
                    //Price = decimal.Parse(Request.Form["Price"]),
                    Business = "代仓储",
                    ApplicationID = this.ApplicationID,
                    
                    AccountType = Yahv.Underly.AccountType.BankStatement,
                };

                Yahv.Payments.PaymentManager.Erp(this.Admin.ErmAdminID).Received.For(fees.ToArray()).Confirm(entity);
            }
            catch (Exception ex)
            {
                ex.CcsLog("收款到Yahv发生异常(StoreReceiptToYahv)");
                throw ex;
            }
        }


    }

    public class StoreReceiptToYahvAmountModel
    {
        public decimal Amount { get; set; }

        public string ReceivableID { get; set; }
    }

}
