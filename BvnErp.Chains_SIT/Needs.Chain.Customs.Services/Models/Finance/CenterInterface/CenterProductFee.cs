using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterProductFee
    {
        /// <summary>
        /// 付款流水号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 收款账户金额
        /// </summary>
        public decimal PayeeAmount { get; set; }
        /// <summary>
        /// 收款账户币种
        /// </summary>
        public string PayeeCurrency { get; set; }       
        /// <summary>
        /// 收款人名称
        /// </summary>
        public string PayeeName { get; set; }
        /// <summary>
        /// 收款人银行名称
        /// </summary>
        public string PayeeBankName { get; set; }        
        /// <summary>
        /// 收款方账户
        /// </summary>
        public string PayeeAccount { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        
        /// <summary>
        /// 付款账户金额
        /// </summary>
        public decimal PayerAmount { get; set; }
        /// <summary>
        /// 付款账户币种
        /// </summary>
        public string PayerCurrency { get; set; }
        /// <summary>
        /// 付款方账户
        /// </summary>
        public string PayerAccount { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime PaymentDate { get; set; }

        public CenterProductFee() { }

        public CenterProductFee(PaymentNotice paymentNotice) 
        {
            if (paymentNotice != null) 
            {
                int paymentType = PaymentTypeTransfer.Current.L2CTransfer(paymentNotice.PayType);
                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == paymentNotice.Admin.ID)?.ErmAdminID;

                this.SeqNo = paymentNotice.SeqNo;
                this.PayeeAmount = paymentNotice.Amount;
                this.PayeeCurrency = paymentNotice.Currency;
                this.PayeeName = paymentNotice.PayeeName;
                this.PayeeAccount = paymentNotice.BankAccount;
                this.PayeeBankName = paymentNotice.BankName;
                this.PaymentDate = DateTime.Now;
                this.PayerAccount = paymentNotice.FinanceAccount.BankAccount;
                this.PaymentType = paymentType;
                this.CreatorID = ErmAdminID;
                if (this.PayeeCurrency == "USD")
                {
                    this.Rate = 1;
                    this.PayerAmount = paymentNotice.Amount;
                    this.PayerCurrency = paymentNotice.Currency;                    
                }
                else 
                {
                    this.Rate = paymentNotice.ExchangeRate.Value;
                    this.PayerAmount = paymentNotice.USDAmount == null ? paymentNotice.Amount : paymentNotice.USDAmount.Value;
                    this.PayerCurrency = "USD";
                }
            }
        }
    }
}
