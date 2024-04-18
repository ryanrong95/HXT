using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 财务付款
    /// </summary>
    public class FinancePayment : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        public string SeqNo { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public Admin Payer { get; set; }

        public FinanceVault FinanceVault { get; set; }

        public FinanceAccount FinanceAccount { get; set; }

        /// <summary>
        /// 收款人名称
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 付款费用类型
        /// </summary>
        public Enums.FinanceFeeType PayFeeType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        public Enums.PaymentType PayType { get; set; }

        public DateTime PayDate { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        //差额(修改后的值 - 原来的值)
        public decimal Difference { get; set; }

        public FinanceAccountFlow FinanceAccountFlow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FeeTypeInt { get; set; }

        #endregion

        public FinancePayment()
        {
            this.Status = Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;

            this.UpdateSuccess += FinancePayment_UpdateSuccess;
            this.DeteleSuccess += FinancePayment_DeteleSuccess;
            this.EnterSuccess += FinancePayment_EnterSuccess;
        }

        public FinancePayment(PaymentNotice notice) : this()
        {
            //赋值
            SeqNo = notice.SeqNo;
            PayeeName = notice.PayeeName;
            BankName = notice.BankName;
            BankAccount = notice.BankAccount;
            Payer = notice.Payer;
            PayFeeType = notice.PayFeeType;
            FinanceAccount = notice.FinanceAccount;
            FinanceVault = notice.FinanceVault;
            if (notice.USDAmount != null) 
            {
                Amount = notice.USDAmount.Value;
                Currency = "USD";
            }
            else 
            {
                Amount = notice.Amount;
                Currency = notice.Currency;
            }
           
            ExchangeRate = notice.ExchangeRate.Value;
            PayType = notice.PayType;
            PayDate = notice.PayDate;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public event FinancePaymentUpdateHanlder UpdateSuccess;
        public event FinancePaymentDeleteHanlder DeteleSuccess;

        private void FinancePayment_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var finance = (FinancePayment)e.Object;

            //更新账户余额
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Balance = finance.FinanceAccount.Balance - finance.Amount
                        }, item => item.ID == finance.FinanceAccount.ID);
            }

            //新增账户流水
            var flow = new FinanceAccountFlow(finance);
            flow.Enter();

           
        }

        private void FinancePayment_UpdateSuccess(object sender, FinancePaymentUpdateEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //原账户流水记录
                var sourceFlow = e.FinancePayment.FinanceAccountFlow;
                if (sourceFlow == null)
                {
                    return;
                }
                //原账户流水之后的所有账户流水的记录
                var flows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                            .Where(item => item.CreateDate > sourceFlow.CreateDate).OrderBy(item => item.CreateDate);
                //账户变化
                if (sourceFlow.FinanceAccount.ID != e.FinancePayment.FinanceAccount.ID)
                {
                    var accounts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();
                    //更新原账户余额数据
                    var account = accounts.Where(item => item.ID == sourceFlow.FinanceAccount.ID).FirstOrDefault();
                    account.Balance += sourceFlow.Amount;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Balance = account.Balance,
                        }, item => item.ID == account.ID);
                    //更新新账户余额数据
                    account = accounts.Where(item => item.ID == e.FinancePayment.FinanceAccount.ID).FirstOrDefault();
                    account.Balance -= e.FinancePayment.Amount;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Balance = account.Balance,
                        }, item => item.ID == account.ID);

                    //更新原账户流水之后的原账户流水数据
                    var accountFlows = flows.Where(item => item.FinanceAccountID == sourceFlow.FinanceAccount.ID);
                    foreach (var accountFlow in accountFlows)
                    {
                        accountFlow.AccountBalance += sourceFlow.Amount;
                        reponsitory.Update(accountFlow, item => item.ID == accountFlow.ID);
                    }

                    //更新原账户流水之后的新账户流水数据
                    accountFlows = flows.Where(item => item.FinanceAccountID == e.FinancePayment.FinanceAccount.ID);
                    foreach (var accountFlow in accountFlows)
                    {
                        accountFlow.AccountBalance -= e.FinancePayment.Amount;
                        reponsitory.Update(accountFlow, item => item.ID == accountFlow.ID);
                    }

                    //更新原账户流水数据
                    sourceFlow.SeqNo = e.FinancePayment.SeqNo;
                    sourceFlow.FinanceVault = e.FinancePayment.FinanceVault;
                    sourceFlow.FinanceAccount = e.FinancePayment.FinanceAccount;
                    sourceFlow.FeeType = e.FinancePayment.PayFeeType;
                    sourceFlow.PaymentType = e.FinancePayment.PayType;
                    sourceFlow.Amount = e.FinancePayment.Amount;
                    sourceFlow.Currency = e.FinancePayment.Currency;
                    if (accountFlows.Count() > 0)
                    {
                        sourceFlow.AccountBalance = accountFlows.First().AccountBalance + accountFlows.First().Amount;
                    }
                    else
                    {
                        sourceFlow.AccountBalance = account.Balance;
                    }
                    sourceFlow.Enter();
                }
                else
                {
                    //金额变化
                    if (e.FinancePayment.Difference == 0M)
                    {
                        //更新对应的FinanceAccountFlows
                        sourceFlow.SeqNo = e.FinancePayment.SeqNo;
                        sourceFlow.FinanceVault= e.FinancePayment.FinanceVault;
                        sourceFlow.FinanceAccount = e.FinancePayment.FinanceAccount;
                        sourceFlow.FeeType = e.FinancePayment.PayFeeType;
                        sourceFlow.PaymentType = e.FinancePayment.PayType;
                        sourceFlow.Currency = e.FinancePayment.Currency;
                        sourceFlow.Enter();
                    }
                    else
                    {
                        //更新对应FinanceAccountFlows以及其后生成的所有记录的账户余额，
                        sourceFlow.SeqNo = e.FinancePayment.SeqNo;
                        sourceFlow.FinanceVault = e.FinancePayment.FinanceVault;
                        sourceFlow.FinanceAccount = e.FinancePayment.FinanceAccount;
                        sourceFlow.FeeType = e.FinancePayment.PayFeeType;
                        sourceFlow.PaymentType = e.FinancePayment.PayType;
                        sourceFlow.Amount = e.FinancePayment.Amount;
                        sourceFlow.Currency = e.FinancePayment.Currency;
                        sourceFlow.AccountBalance -= e.FinancePayment.Difference;
                        sourceFlow.Enter();
                        //更新其后生成的所有流水记录的账户余额
                        var accountFlows = flows.Where(item => item.FinanceAccountID == sourceFlow.FinanceAccount.ID);
                        foreach (var accountFlow in accountFlows)
                        {
                            //减去差额
                            accountFlow.AccountBalance -= e.FinancePayment.Difference;
                            //reponsitory.Update(accountFlow, item => item.ID == accountFlow.ID);
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(
                            new
                            {
                                UpdateDate = DateTime.Now,
                                AccountBalance = accountFlow.AccountBalance,
                            }, item => item.ID == accountFlow.ID);
                        }
                        //更新对应账户余额
                        var account = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>()
                            .Where(item => item.ID == e.FinancePayment.FinanceAccount.ID).FirstOrDefault();
                        account.Balance -= e.FinancePayment.Difference;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Balance = account.Balance,
                        }, item => item.ID == account.ID);
                    }
                }
            }
        }

        private void FinancePayment_DeteleSuccess(object sender, FinancePaymentDeleteEventArgs e)
        {
            //需更新对应FinanceAccountFlows以及其后生的所有记录的账户余额，
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //原账户流水记录
                var sourceFlow = e.FinancePayment.FinanceAccountFlow;
                if (sourceFlow == null)
                {
                    return;
                }
                //原账户流水之后的所有账户流水的记录
                var flows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                            .Where(item => item.CreateDate > sourceFlow.CreateDate);

                sourceFlow.Abandon();
                var accountFlows = flows.Where(item => item.FinanceAccountID == sourceFlow.FinanceAccount.ID);
                foreach (var accountFlow in accountFlows)
                {
                    accountFlow.AccountBalance += sourceFlow.Amount;
                    reponsitory.Update(accountFlow, item => item.ID == accountFlow.ID);
                }
                //更新对应账户余额
                var account = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>()
                    .Where(item => item.ID == e.FinancePayment.FinanceAccount.ID).FirstOrDefault();
                account.Balance += sourceFlow.Amount;
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                new
                {
                    UpdateDate = DateTime.Now,
                    Balance = account.Balance,
                }, item => item.ID == account.ID);
            }
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePayments>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinancePayment);
                        reponsitory.Insert(this.ToLinq());

                        this.OnEnterSuccess();
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);

                        this.OnUpdateSuccess();
                    }
                }
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

        virtual public void OnUpdateSuccess()
        {
            if (this != null && this.UpdateSuccess != null)
            {
                //成功后触发事件
                this.UpdateSuccess(this, new FinancePaymentUpdateEventArgs(this));
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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePayments>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete,
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess();
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
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
            if (this != null && this.DeteleSuccess != null)
            {
                this.DeteleSuccess(this, new FinancePaymentDeleteEventArgs(this));
            }
        }
    }

    /// <summary>
    /// FinancePayment 中 IsPaperInvoiceUpload 字段设置
    /// </summary>
    public class FinancePaymentInvoiceUpload
    {
        private string FinancePaymentID { get; set; }

        public FinancePaymentInvoiceUpload(string financePaymentID)
        {
            this.FinancePaymentID = financePaymentID;
        }

        /// <summary>
        /// 设为已上传
        /// </summary>
        public void SetUploaded()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePayments>(new
                {
                    IsPaperInvoiceUpload = (int)Enums.UploadStatus.Uploaded,
                }, item => item.ID == this.FinancePaymentID);
            }
        }

        /// <summary>
        /// 设为未上传
        /// </summary>
        public void SetUnUpload()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePayments>(new
                {
                    IsPaperInvoiceUpload = (int)Enums.UploadStatus.NotUpload,
                }, item => item.ID == this.FinancePaymentID);
            }
        }

    }

}
