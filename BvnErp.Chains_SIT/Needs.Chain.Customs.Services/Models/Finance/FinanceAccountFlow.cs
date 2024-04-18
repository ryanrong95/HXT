using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Hanlders.Finance;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 金库账户
    /// </summary>
    public class FinanceAccountFlow : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        /// <summary>
        /// 收/付款人
        /// </summary>
        public Admin Admin { get; set; }

        public string SeqNo { get; set; }

        public string SourceID { get; set; }

        public FinanceVault FinanceVault { get; set; }

        public FinanceAccount FinanceAccount { get; set; }

        public Enums.FinanceType Type { get; set; }

        public Enums.FinanceFeeType FeeType { get; set; }

        public Enums.PaymentType PaymentType { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal AccountBalance { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 对方户名
        /// </summary>
        public string OtherAccount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FeeTypeInt { get; set; }

        #endregion

        public FinanceAccountFlow()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.AmountUpdated += FinanceAccountFlow_AmountUpdated;
            this.AbandonSuccess += FinanceAccountFlow_AbandonSuccess;
        }

        public FinanceAccountFlow(FinancePayment payment) : this()
        {
            Admin = payment.Payer;
            SeqNo = payment.SeqNo;
            SourceID = payment.ID;
            FinanceVault = payment.FinanceVault;
            FinanceAccount = payment.FinanceAccount;
            Type = Enums.FinanceType.Payment;
            FeeType = payment.PayFeeType;
            PaymentType = payment.PayType;
            Amount = payment.Amount;
            Currency = payment.Currency;
            AccountBalance = payment.FinanceAccount.Balance - Amount;
        }

        public FinanceAccountFlow(FinanceReceipt receipt) : this()
        {
            Admin = receipt.Admin;
            SeqNo = receipt.SeqNo;
            SourceID = receipt.ID;
            FinanceVault = receipt.Vault;
            FinanceAccount = receipt.Account;
            Type = Enums.FinanceType.Receipt;
            FeeType = receipt.FeeType;
            PaymentType = receipt.ReceiptType;
            Amount = receipt.Amount;
            Currency = receipt.Currency;
            AccountBalance = receipt.Account.Balance + Amount;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event AccountFlowAmountUpdatedHanlder AmountUpdated;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //主键ID（FinanceAccount +8位年月日+6位流水号）
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinanceAccountFlow);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 收款流水更新
        /// </summary>
        /// <param name="receipt">收款记录</param>
        public void Update(FinanceReceipt receipt)
        {
            this.SeqNo = receipt.SeqNo;
            this.FinanceVault = receipt.Vault;
            this.FinanceAccount = receipt.Account;
            this.FeeType = receipt.FeeType;
            this.PaymentType = receipt.ReceiptType;
            this.Amount = receipt.Amount;
            this.Currency = receipt.Currency;
            this.Enter();

            if (receipt.Difference != 0)
            {
                this.OnAmountUpdated(receipt.Difference);
            }
        }

        /// <summary>
        /// 付款流水更新
        /// </summary>
        /// <param name="payment">付款记录</param>
        public void Update(FinancePayment payment)
        {
            this.SeqNo = payment.SeqNo;
            this.FinanceVault = payment.FinanceVault;
            this.FinanceAccount = payment.FinanceAccount;
            this.FeeType = payment.PayFeeType;
            this.PaymentType = payment.PayType;
            this.Amount = payment.Amount;
            this.Currency = payment.Currency;
            this.Enter();

            if (payment.Difference != 0)
            {
                this.OnAmountUpdated(payment.Difference);
            }
        }

        virtual protected void OnAmountUpdated(decimal difference)
        {
            if (this != null && this.AmountUpdated != null)
            {
                //成功后触发事件
                this.AmountUpdated(this, new AccountFlowAmountUpdatedEventArgs(this, difference));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(new
                    {
                        UpdateDate = DateTime.Now,
                        Status = Enums.Status.Delete
                    }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        private void FinanceAccountFlow_AmountUpdated(object sender, AccountFlowAmountUpdatedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var flow = e.FinanceAccountFlow;
                var difference = e.Difference;
                var afterCreatedFlows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                                        .Where(item => item.FinanceAccountID == flow.FinanceAccount.ID && item.CreateDate > flow.CreateDate);

                //更新其后生成的所有流水记录的账户余额
                var accountFlows = afterCreatedFlows.Where(item => item.FinanceAccountID == flow.FinanceAccount.ID);
                foreach (var accountFlow in accountFlows)
                {
                    if (flow.Type == Enums.FinanceType.Receipt)
                    {
                        accountFlow.AccountBalance += difference; //加上差额
                    }
                    else
                    {
                        accountFlow.AccountBalance -= difference; //减掉差额
                    }
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(new
                    {
                        UpdateDate = DateTime.Now,
                        AccountBalance = accountFlow.AccountBalance,
                    }, item => item.ID == accountFlow.ID);
                }
            }
        }

        private void FinanceAccountFlow_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var flow = (FinanceAccountFlow)e.Object;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //该账户流水之后产生的所有账户流水记录
                var flows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                            .Where(item => item.FinanceAccountID == flow.FinanceAccount.ID && item.CreateDate > flow.CreateDate);
                //变更余额
                foreach (var accountFlow in flows)
                {
                    decimal accountBalance;
                    if (flow.Type == Enums.FinanceType.Receipt)
                    {
                        accountBalance = accountFlow.AccountBalance - flow.Amount;
                    }
                    else
                    {
                        accountBalance = accountFlow.AccountBalance + flow.Amount;
                    }
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(new
                    {
                        UpdateDate = DateTime.Now,
                        AccountBalance = accountBalance,
                    }, item => item.ID == accountFlow.ID);
                }
            }
        }
    }
}
