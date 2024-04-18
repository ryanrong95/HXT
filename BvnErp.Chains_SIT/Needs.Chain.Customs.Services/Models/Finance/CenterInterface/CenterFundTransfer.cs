using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public  class CenterFundTransfer
    {
        /// <summary>
        /// 调出账号
        /// </summary>
        public string OutAccountNo { get; set; }
        /// <summary>
        /// 调出金额
        /// </summary>
        public decimal OutAmount { get; set; }
        /// <summary>
        /// 调出币制
        /// </summary>
        public string OutCurrency { get; set; }
        /// <summary>
        /// 调出流水号
        /// </summary>
        public string OutSeqNo { get; set; }
        /// <summary>
        /// 调入账号
        /// </summary>
        public string InAccountNo { get; set; }
        /// <summary>
        /// 调入金额
        /// </summary>
        public decimal InAmount { get; set; }
        /// <summary>
        /// 调入币制
        /// </summary>
        public string InCurrency { get; set; }       
        /// <summary>
        /// 调入流水号
        /// </summary>
        public string InSeqNo { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        public int FeeType { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 贴现流水号
        /// </summary>
        public string DiscountSeqNo { get; set; }
        /// <summary>
        /// 贴现利息
        /// </summary>
        public decimal? DiscountInterest { get; set; }
        /// <summary>
        /// 手续费流水号
        /// </summary>
        public string PoundageSeqNo { get; set; }
        /// <summary>
        /// 收费费
        /// </summary>
        public decimal? Poundage { get; set; }
        public decimal? QRCodeFee { get; set; }
        public string QRCodeFeeSeqNo { get; set; }

        public CenterFundTransfer() { }
        public CenterFundTransfer(FundTransferApplies fundTransferApplies)
        {
            if (fundTransferApplies != null)
            {
                this.OutAccountNo = fundTransferApplies.OutAccount.BankAccount;
                this.OutAmount = fundTransferApplies.OutAmount;
                this.OutCurrency = fundTransferApplies.OutCurrency;
                this.OutSeqNo = fundTransferApplies.OutSeqNo;

                this.InAccountNo = fundTransferApplies.InAccount.BankAccount;
                this.InAmount = fundTransferApplies.InAmount;
                this.InCurrency = fundTransferApplies.InCurrency;
                this.InSeqNo = fundTransferApplies.InSeqNo;

                this.Rate = fundTransferApplies.Rate;
                int paymentType = PaymentTypeTransfer.Current.L2CTransfer(fundTransferApplies.PaymentType);
                this.PaymentType = paymentType;
                this.FeeType = (int)fundTransferApplies.FeeType;
                this.PaymentDate = fundTransferApplies.PaymentDate;

                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == fundTransferApplies.Admin.ID)?.ErmAdminID;
                this.CreatorID = ErmAdminID;

                this.DiscountInterest = fundTransferApplies.DiscountInterest;
                this.DiscountSeqNo = fundTransferApplies.FromSeqNo;
                this.Poundage = fundTransferApplies.Poundage;
                this.PoundageSeqNo = fundTransferApplies.PoundageSeqNo;
                this.QRCodeFee = fundTransferApplies.QRCodeFee;
                if (this.QRCodeFee != null)
                {
                    this.QRCodeFeeSeqNo = "QRFee" + fundTransferApplies.OutSeqNo;
                }                
            }
        }
    }
}
