using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Payments.Views;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 数字账户
    /// </summary>
    public class DigitalAccount
    {
        ConcurrentDictionary<Currency, DigitalSubtotal> subTotals;
        ConcurrentDictionary<string, DigitalSubtotal> subCodeTotals;
        PayInfo payInfo;


        internal DigitalAccount(PayInfo payInfo)
        {
            this.payInfo = payInfo;
            this.subTotals = new ConcurrentDictionary<Currency, DigitalSubtotal>();
            this.subCodeTotals = new ConcurrentDictionary<string, DigitalSubtotal>();

            using (var flowView = new FlowAccountsStatisticsView())
            using (var bankView = new BankFlowAccountsView())
            {
                foreach (var view in flowView.Where(item => item.Payee == payInfo.Payee
                                                            && item.Payer == payInfo.Payer
                                                            && item.Type == AccountType.BankStatement))
                {
                    this[view.Currency].Available = view.Price;
                    this[view.Currency].Currency = view.Currency;
                }

                foreach (var view in bankView.Where(item => item.Payee == payInfo.Payee
                                                            && item.Payer == payInfo.Payer))
                {
                    this[view.FormCode].Available = view.Price;
                    this[view.FormCode].Currency = view.Currency;
                }
            }
        }


        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DigitalSubtotal this[Currency index]
        {
            get
            {
                return this.subTotals.GetOrAdd(index, new DigitalSubtotal
                {
                    Currency = Currency.Unknown,
                    Available = 0
                });
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DigitalSubtotal this[string code]
        {
            get
            {
                return this.subCodeTotals.GetOrAdd(code, new DigitalSubtotal
                {
                    Currency = Currency.Unknown,
                    Available = 0
                });
            }
        }

        /// <summary>
        /// 充值 
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="bank">收款银行</param>
        /// <param name="account">银行账号</param>
        /// <param name="formCode">收款流水</param>
        /// <remarks>
        /// </remarks>
        public string Recharge(Currency currency, decimal price, string bank, string account, string formCode, DateTime receiptDate, DateTime? createTime = null)
        {
            string id = string.Empty;

            if (price <= 0)
            {
                throw new Exception("金额不能为小于等于0!");
            }

            if (string.IsNullOrWhiteSpace(formCode))
            {
                throw new Exception("银行流水号不能为空!");
            }


            var rate = ExchangeRates.Universal[currency, Currency.CNY];

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            using (var flowsView = new FlowAccountsTopView(reponsitory))
            {
                //if (flowsView.Any(item => item.FormCode == formCode))
                //{
                //    throw new Exception("银行流水号重复!");
                //}

                id = PKeySigner.Pick(PKeyType.FlowAccount);
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                {
                    ID = id,
                    Currency = (int)currency,
                    AdminID = this.payInfo.Inputer.ID,
                    Business = this.payInfo.Conduct,
                    Payee = this.payInfo.Payee,
                    Payer = this.payInfo.Payer,
                    Type = (int)AccountType.BankStatement,
                    Price = price,
                    CreateDate = createTime ?? DateTime.Now,
                    Currency1 = (int)Currency.CNY,
                    ERate1 = rate,
                    Price1 = price * rate,
                    FormCode = formCode,
                    Bank = bank,
                    Account = account,
                    ReceiptDate = receiptDate,
                });
            }

            return id;
        }

        /// <summary>
        /// 预收账款 
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="bank">收款银行</param>
        /// <param name="account">银行账号</param>
        /// <param name="formCode">收款流水</param>
        /// <param name="receiptDate">收款日期</param>
        /// <remarks>
        /// </remarks>
        public string AdvanceFromCustomers(Currency currency, decimal price, string bank, string account, string formCode, DateTime receiptDate)
        {
            string id = string.Empty;
            try
            {
                id = this.Recharge(currency, price, bank, account, formCode, receiptDate);
                Oplogs.Oplog(this.payInfo.Inputer.ID, typeof(DigitalAccount).FullName, "Pays", "预收账款", $"AdvanceFromCustomers(currency:{currency}, price:{price}, bank:{bank}, account:{account}, formCode:{formCode}, receiptDate:{receiptDate})", "");
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.payInfo.Inputer.ID, typeof(DigitalAccount).FullName, ex, remark: $"AdvanceFromCustomers(currency:{currency}, price:{price}, bank:{bank}, account:{account}, formCode:{formCode}, receiptDate:{receiptDate})");
                throw ex;
            }
            return id;
        }

        /// <summary>
        /// 预付账款 
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="bank">收款银行</param>
        /// <param name="account">银行账号</param>
        /// <param name="formCode">打款流水</param>
        /// <param name="receiptDate">打款日期</param>
        /// <remarks>
        /// </remarks>
        public string AdvanceToSuppliers(Currency currency, decimal price, string bank, string account, string formCode, DateTime receiptDate)
        {
            string id = string.Empty;
            try
            {
                id = this.Recharge(currency, price, bank, account, formCode, receiptDate);
                Oplogs.Oplog(this.payInfo.Inputer.ID, typeof(DigitalAccount).FullName, "Pays", "预收账款", $"AdvanceToSuppliers(currency:{currency}, price:{price}, bank:{bank}, account:{account}, formCode:{formCode}, receiptDate:{receiptDate})", "");
            }
            catch (Exception ex)
            {
                Oplogs.Logs_Error(this.payInfo.Inputer.ID, typeof(DigitalAccount).FullName, ex, remark: $"AdvanceToSuppliers(currency:{currency}, price:{price}, bank:{bank}, account:{account}, formCode:{formCode}, receiptDate:{receiptDate})");
                throw ex;
            }
            return id;
        }
    }
}