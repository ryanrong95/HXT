using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AcceptanceBill:IUnique
    {
        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 商业承兑、银行承兑
        /// </summary>
        public Enums.MoneyOrderType Type { get; set; }
        /// <summary>
        /// 票据号码
        /// </summary>
        public string Code { get; set; }
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
        public FinanceAccount PayerAccount { get; set; }
        /// <summary>
        /// 收票人账户
        /// </summary>
        public FinanceAccount PayeeAccount { get; set; }
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
        /// <summary>
        /// 签收日期
        /// </summary>
        public DateTime? AcceptedDate { get; set; }
        /// <summary>
        /// 背书人
        /// </summary>
        public string Endorser { get; set; }
        public Enums.MoneyOrderStatus BillStatus { get; set; }
        public Admin Creator { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        /// <summary>
        /// 是否生成承兑凭证
        /// </summary>
        public bool? AccCreSta { get; set; }
        /// <summary>
        /// 承兑凭证字
        /// </summary>
        public string AccCreWord { get; set; }
        /// <summary>
        /// 承兑凭证号
        /// </summary>
        public string AccCreNo { get; set; }
        /// <summary>
        ///  是否生成贴现凭证
        /// </summary>
        public bool? BuyCreSta { get; set; }
        /// <summary>
        /// 贴现凭证字
        /// </summary>
        public string BuyCreWord { get; set; }
        /// <summary>
        /// 贴现凭证号
        /// </summary>
        public string BuyCreNo { get; set; }
        public string ReceiveBank { get; set; }
        public string RequestID { get; set; }
        public string FundTransferAccountName { get; set; }
        public string BuyRequestID { get; set; }

        #endregion

        public AcceptanceBill()
        {
            this.Currency = "RMB";
            this.BillStatus = Enums.MoneyOrderStatus.Ticketed;
            this.Type = Enums.MoneyOrderType.Bank;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MoneyOrders>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.MoneyOrders
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        Code = this.Code,
                        Name = this.Name,
                        BankCode = this.BankCode,
                        BankName = this.BankName,
                        BankNo = this.BankNo,
                        Currency = this.Currency,
                        Price = this.Price,
                        PayeeAccountID = this.PayeeAccount.ID,
                        PayerAccountID = this.PayerAccount.ID,

                        IsTransfer = this.IsTransfer,
                        IsMoney = this.IsMoney,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        Nature = (int)this.Nature,
                        ExchangeDate = this.ExchangeDate,
                        ExchangePrice = this.ExchangePrice,
                        CreatorID = this.Creator.ID,
                        Endorser = this.Endorser,
                        BillStatus = (int)this.BillStatus,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.MoneyOrders>(
                        new {
                            Type = (int)this.Type,
                            Code = this.Code,
                            Name = this.Name,
                            BankCode = this.BankCode,
                            BankName = this.BankName,
                            BankNo = this.BankNo,
                            Currency = this.Currency,
                            Price = this.Price,
                            PayeeAccountID = this.PayeeAccount.ID,
                            PayerAccountID = this.PayerAccount.ID,

                            IsTransfer = this.IsTransfer,
                            IsMoney = this.IsMoney,
                            StartDate = this.StartDate,
                            EndDate = this.EndDate,
                            Nature = (int)this.Nature,
                            ExchangeDate = this.ExchangeDate,
                            ExchangePrice = this.ExchangePrice,
                            CreatorID = this.Creator.ID,
                            Endorser = this.Endorser,
                            BillStatus = (int)this.BillStatus,
                            Status = (int)this.Status,                           
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
                }
            }
        }

        public void UpdateBillStatus(DateTime exchangeDate,decimal exchangePrice)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.MoneyOrders>(
                        new
                        {                           
                            BillStatus = (int)Enums.MoneyOrderStatus.Exchanged,                            
                            UpdateDate = DateTime.Now,
                            ExchangeDate = exchangeDate,
                            ExchangePrice = exchangePrice
                        }, item => item.ID == this.ID);
            }
        }
    }
}
