using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 中心收款同步
    /// </summary>
    public class CenterFinanceReceipt
    {
       
        /// <summary>
        /// 收款流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 原来收款流水号
        /// </summary>
        public string OldSeqNo { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 财务收款类型：与中心保持一致，传ID
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 收款方式：与中心保持一致，传枚举值value
        /// </summary>
        public int ReceiptType { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 金库
        /// </summary>
        public string Vault { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 收款人/财务
        /// </summary>
        public string CreatorID { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 账户性质 个人账户，公司账户，未知 与中心保持一致
        /// </summary>
        public int AccountSource { get; set; }

        public CenterFinanceReceipt() { }

        public CenterFinanceReceipt(FinanceReceipt receipt)
        {
            if (receipt != null)
            {
                string ceterFeetype = FeeTypeTransfer.Current.L2CInTransfer(receipt.FeeType);
                int paymentType = PaymentTypeTransfer.Current.L2CTransfer(receipt.ReceiptType);

                this.SeqNo = receipt.SeqNo;
                this.Payer = receipt.Payer;
                this.FeeType = ceterFeetype;
                this.ReceiptType = paymentType;
                this.ReceiptDate = receipt.ReceiptDate;
                this.Currency = receipt.Currency;
                this.Amount = receipt.Amount;
                this.Rate = receipt.Rate;
                this.Summary = receipt.Summary;
                this.AccountSource = (int)receipt.AccountProperty;
                //this.CreatorID = receipt.Admin.ErmAdminID;
                this.CreatorID = "Admin00530";

                this.Vault = receipt.Vault.Name;
                this.Account = receipt.Account.BankAccount;
            }
            
        }
    }
}
