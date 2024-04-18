using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterAcceptanceBill
    {
        #region 属性
     
        /// <summary>
        /// 商业承兑、银行承兑
        /// </summary>
        public Enums.MoneyOrderType Type { get; set; }
        /// <summary>
        /// 票据号码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 更新使用
        /// </summary>
        public string OldCode { get; set; }
        /// <summary>
        /// 承兑人信息-全称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 承兑人信息-银行账号
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 承兑人信息-开户行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 承兑人信息-开户行行号
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 汇票金额
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 是否允许背书转让
        /// </summary>
        public bool? IsTransfer { get; set; }
        /// <summary>
        /// 是否能贴现
        /// </summary>
        public bool? IsMoney { get; set; }
        /// <summary>
        /// 出票人账户
        /// </summary>
        public string PayerAccountNo { get; set; }
        /// <summary>
        /// 收票人账户
        /// </summary>
        public string PayeeAccountNo { get; set; }
        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 汇票到期日
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 承兑性质 电子承兑、纸质承兑
        /// </summary>
        public Enums.MoneyOrderNature Nature { get; set; }
        /// <summary>
        /// 兑换日期
        /// </summary>
        public DateTime? ExchangeDate { get; set; }
        /// <summary>
        /// 兑换金额
        /// </summary>
        public decimal? ExchangePrice { get; set; }
        public Enums.MoneyOrderStatus Status { get; set; }
        public string CreatorID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Endorser { get; set; }
        public List<CenterFeeFile> Files { get; set; }
        #endregion

        public CenterAcceptanceBill(AcceptanceBill bill)
        {
            if (bill != null)
            {
                this.Files = new List<CenterFeeFile>();
                this.Type = bill.Type;
                this.Code = bill.Code;
                this.Name = bill.Name;
                this.BankCode = bill.BankCode;
                this.BankName = bill.BankName;
                this.BankNo = bill.BankNo;
                this.Currency = bill.Currency;
                this.Price = bill.Price;                
                //this.PayeeAccountNo = bill.PayeeAccount.BankAccount;
                //this.PayerAccountNo = bill.PayerAccount.BankAccount;
                var PayerAccout = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.ID == bill.PayerAccount.ID).FirstOrDefault();
                if (PayerAccout != null)
                {
                    this.PayerAccountNo = PayerAccout.BankAccount;
                }
                var PayeeAccout = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.ID == bill.PayeeAccount.ID).FirstOrDefault();
                if (PayeeAccout != null)
                {
                    this.PayeeAccountNo = PayeeAccout.BankAccount;
                }
                this.IsTransfer = bill.IsTransfer;
                this.IsMoney = bill.IsMoney;
                this.StartDate = bill.StartDate;
                this.EndDate = bill.EndDate;
                this.Nature = bill.Nature;
                this.ExchangeDate = bill.ExchangeDate;
                this.ExchangePrice = bill.ExchangePrice;
                this.Status = bill.BillStatus;
                this.CreatorID = "Admin00530";
                this.CreateDate = DateTime.Now;
                this.Endorser = bill.Endorser;
            }            
        }
    }
}
