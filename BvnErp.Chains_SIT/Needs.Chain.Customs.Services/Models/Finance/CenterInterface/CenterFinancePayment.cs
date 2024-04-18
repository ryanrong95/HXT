using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterFinancePayment
    {
        /// <summary>
        /// 费用类型-固定货款
        /// </summary>
        public string FeeType { get; set; }        
        /// <summary>
        /// 收款方账号
        /// </summary>
        public string InAccountNo { get; set; }
        /// <summary>
        /// 中间账号
        /// </summary>
        public string MidAccountNo { get; set; }
        /// <summary>
        /// 付款账号
        /// </summary>
        public string OutAccountNo { get; set; }
        /// <summary>
        /// RMB总金额
        /// </summary>
        public decimal RMBAmount { get; set; }
        /// <summary>
        /// 外币币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 外币总金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 收款流水号
        /// </summary>
        public string InSeqNo { get; set; }
        /// <summary>
        /// 付款流水号
        /// </summary>
        public string OutSeqNo { get; set; }
        /// <summary>
        /// 中间收款流水号
        /// </summary>
        public string MidInSeqNo { get; set; }
        /// <summary>
        /// 中间付款流水号
        /// </summary>
        public string MidOutSeqNo { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal? Poundage { get; set; }
        /// <summary>
        /// 手续费流水号
        /// </summary>
        public string PoundageSeqNo { get; set; }
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
       
        public CenterFinancePayment()
        { }

        public CenterFinancePayment(SwapNotice payment)
        {
            if (payment != null)
            {
                string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(Enums.FinanceFeeType.PayGoods);
                int paymentType = PaymentTypeTransfer.Current.L2CTransfer(Enums.PaymentType.TransferAccount);

                this.FeeType = ceterFeetype;
                this.InAccountNo = payment.InAccount.BankAccount;
                this.MidAccountNo = payment.MidAccount == null ? "" : payment.MidAccount.BankAccount;
                this.OutAccountNo = payment.OutAccount.BankAccount;
                this.InSeqNo = payment.SeqNoIn;
                this.OutSeqNo = payment.SeqNoOut;
                this.MidInSeqNo = payment.SeqNoMidR;
                this.MidOutSeqNo = payment.SeqNoMidP;

                this.RMBAmount = payment.TotalAmountCNY;
                this.Amount = payment.TotalAmount;
                this.Currency = payment.Currency;
                this.Rate = payment.ExchangeRate;

                this.Poundage = payment.Poundage;
                this.PoundageSeqNo = payment.SeqNoPoundage;

                this.PaymentType = paymentType;
                this.PaymentDate = payment.UpdateDate;

                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == payment.Admin.ID)?.ErmAdminID;
                this.CreatorID = ErmAdminID;
            }
        }
    }
}
