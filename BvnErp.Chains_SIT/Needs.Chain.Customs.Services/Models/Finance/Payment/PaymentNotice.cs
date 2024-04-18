using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models.HttpUtility;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付款申请
    /// </summary>
    public class PaymentNotice : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Admin Admin { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public Admin Payer { get; set; }
        /// <summary>
        /// 付款申请
        /// </summary>
        public PaymentApply PaymentApply { get; set; }

        /// <summary>
        /// 付款申请
        /// </summary>
        public PayExchangeApply PayExchangeApply { get; set; }

        public FinanceVault FinanceVault { get; set; }

        public FinanceAccount FinanceAccount { get; set; }

        public Enums.FinanceFeeType PayFeeType { get; set; }
        /// <summary>
        /// 费用描述
        /// </summary>
        public string FeeDesc { get; set; }

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
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 美元金额
        /// </summary>
        public decimal? USDAmount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        public decimal? ExchangeRate { get; set; }

        public DateTime PayDate { get; set; }

        public Enums.PaymentType PayType { get; set; }

        public Enums.PaymentNoticeStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 审批备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FeeTypeInt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CostApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal? Poundage { get; set; }

        /// <summary>
        /// 手续费流水号
        /// </summary>
        public string SeqNoPoundage { get; set; } = string.Empty;

        public string RefundApplyID { get; set; } = string.Empty;

        #endregion

        private PaymentNoticeFile file;

        public PaymentNoticeFile File
        {
            get
            {
                if (file == null)
                {
                    using (var view = new Views.PaymentNoticeFileView())
                    {
                        var query = view.Where(item => item.PaymentNoticeID == this.ID);
                        this.file = query.FirstOrDefault();
                    }
                }
                return this.file;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.file = value;
            }
        }

        /// <summary>
        /// 付款通知明细项
        /// </summary>
        PaymentNoticeItems items;
        public PaymentNoticeItems PaymentItems
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.PaymentNoticeItemView())
                    {
                        var query = view.Where(item => item.PaymentNoticeID == this.ID);
                        this.PaymentItems = new PaymentNoticeItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.items = new PaymentNoticeItems(value, new Action<PaymentNoticeItem>(delegate (PaymentNoticeItem item)
                {
                    item.PaymentNoticeID = this.ID;
                }));
            }
        }

        private Admin Operator;
        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        public Admin GetOperator()
        {
            return this.Operator;
        }

        public PaymentNotice()
        {
            this.Status = Enums.PaymentNoticeStatus.UnPay;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.PaymentNoticePaid += PaymentNotice_PaymentNoticePaid;
            this.EnterSuccessSendNotice += EnterSuccess_SendNotice;
            this.PaymentNotice2Center += PaymentNotice_PaymentNotice2Center;
            this.PaymentNotice2Dyj += PaymentNotice_PaymentNotice2Dyj;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event PaymentNoticePaidHanlder EnterSuccessSendNotice;

        /// <summary>
        /// 付款完成
        /// </summary>
        public event PaymentNoticePaidHanlder PaymentNoticePaid;
        public event PaymentNoticePaidHanlder PaymentNotice2Center;
        public event PaymentNoticePaidHanlder PaymentNotice2Dyj;

        private void PaymentNotice_PaymentNoticePaid(object sender, PaymentNoticePaidEventArgs e)
        {
            var notice = e.PaymentNotice;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var financePayment = new FinancePayment(e.PaymentNotice);
                //手续费
                if (notice.Poundage != null)
                {
                    FinancePayment Poundage = new FinancePayment();
                    Poundage.SeqNo = notice.SeqNoPoundage;
                    Poundage.PayeeName = notice.BankName;
                    Poundage.Payer = notice.Operator;
                    Poundage.PayFeeType = FinanceFeeType.Poundage;
                    Poundage.FinanceVault = notice.FinanceVault;
                    Poundage.FinanceAccount = notice.FinanceAccount;
                    Poundage.BankName = notice.BankName;
                    Poundage.BankAccount = notice.BankAccount;
                    Poundage.Amount = notice.Poundage.Value;
                    Poundage.Currency = notice.Currency;
                    Poundage.ExchangeRate = notice.ExchangeRate.Value;
                    Poundage.PayType = PaymentType.TransferAccount;
                    Poundage.PayDate = notice.PayDate;
                    Poundage.Enter();
                    Poundage.FinanceAccount.Balance = notice.FinanceAccount.Balance - notice.Poundage.Value;
                }
                //财务付款
                financePayment.Enter();
                //更新付汇申请的状态
                var payExchangeApply = e.PaymentNotice.PayExchangeApply;
                if (payExchangeApply != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Completed
                        }, item => item.ID == payExchangeApply.ID);
                    if (e.PaymentNotice.Operator != null)
                    {
                        payExchangeApply.Log(e.PaymentNotice.Operator, "财务[" + e.PaymentNotice.Operator.ByName + "]完成付款");
                    }
                    //payExchangeApply.SetOperator(e.PaymentNotice.Operator);
                    //payExchangeApply.Pay();
                }

                //如果 CostApplyID 不为空，则更新 CostApply 状态
                string costApplyID = e.PaymentNotice.CostApplyID;
                if (!string.IsNullOrEmpty(costApplyID))
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                    {
                        CostStatus = (int)Enums.CostStatusEnum.PaySuccess,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == costApplyID);

                    string summary = string.Empty;
                    if (e.PaymentNotice.Operator != null)
                    {
                        summary = "财务[" + e.PaymentNotice.Operator.ByName + "]完成付款";
                    }
                    else
                    {
                        summary = "财务完成付款";
                    }

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyLogs
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CostApplyID = costApplyID,
                        AdminID = e.PaymentNotice.Operator?.ID,
                        CurrentCostStatus = (int)Enums.CostStatusEnum.UnPay,
                        NextCostStatus = (int)Enums.CostStatusEnum.PaySuccess,
                        CreateDate = DateTime.Now,
                        Summary = summary,
                    });
                }

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
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PaymentNotice);
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
            if (this != null)
            {
                this.EnterSuccessSendNotice(this, new PaymentNoticePaidEventArgs(this));
            }

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void EnterSuccess_SendNotice(object o, PaymentNoticePaidEventArgs e)
        {
            //NoticeLog noticeLog = new NoticeLog();
            //noticeLog.MainID = e.PaymentNotice.ID;
            //noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.PayPayExchange;
            //noticeLog.AdminIDs.Add(e.PaymentNotice.Payer.ID);  //e.PaymentNotice.Payer 为空， .ID 会报错“未将对象引用到对象的实例”
            //noticeLog.Readed = false;
            //noticeLog.SendNotice();
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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.PaymentNoticeStatus.Canceled,
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
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 确认付款
        /// </summary>
        public void Paid()
        {
            //更新通知
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //通知已付款
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNotices>(
                    new
                    {
                        SeqNo = this.SeqNo,
                        ExchangeRate = this.ExchangeRate,
                        PayDate = this.PayDate,
                        PayType = this.PayType,
                        FinanceVaultID = this.FinanceVault.ID,
                        FinanceAccountID = this.FinanceAccount.ID,
                        UpdateDate = DateTime.Now,
                        Status = Enums.PaymentNoticeStatus.Paid,
                        Poundage = this.Poundage,
                        SeqNoPoundage = this.SeqNoPoundage,
                        USDAmount = this.USDAmount,
                        Amount = this.USDAmount == null ? this.Amount : this.USDAmount.Value,
                        Currency = this.USDAmount == null ? this.Currency : "USD",
                    }, item => item.ID == this.ID);

                //修改通知项状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>(
                    new
                    {
                        Status = Enums.PaymentNoticeStatus.Paid
                    }, item => item.PaymentNoticeID == this.ID);
            }
            this.OnPaid(new PaymentNoticePaidEventArgs(this));
            this.Post2Center(new PaymentNoticePaidEventArgs(this));
            if (this.PayExchangeApply != null)
            {
                this.Post2Dyj(new PaymentNoticePaidEventArgs(this));
            }
        }

        virtual protected void OnPaid(PaymentNoticePaidEventArgs args)
        {
            this.PaymentNoticePaid?.Invoke(this, args);
        }

        virtual protected void Post2Center(PaymentNoticePaidEventArgs args)
        {
            this.PaymentNotice2Center?.Invoke(this, args);
        }

        virtual protected void Post2Dyj(PaymentNoticePaidEventArgs args)
        {
            this.PaymentNotice2Dyj?.Invoke(this, args);
        }

        private void PaymentNotice_PaymentNotice2Center(object sender, PaymentNoticePaidEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var paymentNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>().Where(t => t.ID == this.ID).FirstOrDefault();
                if (!string.IsNullOrEmpty(paymentNotice.CostApplyID))
                {
                    CenterFee centerFee = new CenterFee(this, paymentNotice.CostApplyID);

                    SendStrcut sendStrcut = new SendStrcut();
                    sendStrcut.sender = "FSender001";
                    sendStrcut.option = CenterConstant.Enter;
                    sendStrcut.model = centerFee;
                    //提交中心
                    string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                    string requestUrl = URL + FinanceApiSetting.FeeUrl;
                    string apiclient = JsonConvert.SerializeObject(sendStrcut);

                    Logs log = new Logs();
                    log.Name = "费用同步";
                    log.MainID = paymentNotice.ID;
                    log.AdminID = paymentNotice.AdminID;
                    log.Json = apiclient;
                    log.Summary = "";
                    log.Enter();

                    HttpResponseMessage response = new HttpResponseMessage();
                    response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);

                    //如果有手续费，手续费也要通给中心
                    var notice = e.PaymentNotice;
                    if (notice.Poundage != null)
                    {
                        CenterFee centerFeePoundage = new CenterFee();
                        centerFeePoundage.ReceiveAccountNo = notice.BankAccount;
                        centerFeePoundage.AccountNo = notice.FinanceAccount.BankAccount;
                        centerFeePoundage.CreatorID = centerFee.CreatorID;
                        if (string.IsNullOrEmpty(notice.SeqNoPoundage))
                        {
                            centerFeePoundage.SeqNo = " ";
                        }
                        else
                        {
                            centerFeePoundage.SeqNo = notice.SeqNoPoundage;
                        }

                        centerFeePoundage.Amount = notice.Poundage.Value;
                        centerFeePoundage.Currency = notice.Currency;
                        centerFeePoundage.Rate = notice.ExchangeRate == null ? 1 : notice.ExchangeRate.Value;
                        centerFeePoundage.PaymentDate = notice.PayDate;
                        centerFeePoundage.PaymentType = centerFee.PaymentType;
                        centerFeePoundage.MoneyType = centerFee.MoneyType;

                        string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(FinanceFeeType.PayBankPaypal);
                        CenterFeeItem feeItem = new CenterFeeItem();
                        feeItem.FeeType = ceterFeetype;
                        feeItem.Amount = centerFeePoundage.Amount;
                        feeItem.FeeDesc = "手续费";

                        centerFeePoundage.FeeItems = new List<CenterFeeItem>();
                        centerFeePoundage.FeeItems.Add(feeItem);

                        SendStrcut sendStrcutPoundage = new SendStrcut();
                        sendStrcutPoundage.sender = "FSender001";
                        sendStrcutPoundage.option = CenterConstant.Enter;
                        sendStrcutPoundage.model = centerFeePoundage;

                        string apiclientPoundage = JsonConvert.SerializeObject(sendStrcutPoundage);

                        Logs logP = new Logs();
                        logP.Name = "费用同步";
                        logP.MainID = paymentNotice.ID;
                        logP.AdminID = paymentNotice.AdminID;
                        logP.Json = apiclientPoundage;
                        logP.Summary = "银行手续费同步";
                        logP.Enter();

                        HttpResponseMessage responseP = new HttpResponseMessage();
                        responseP = new HttpClientHelp().HttpClient("POST", requestUrl, apiclientPoundage);
                    }
                }
                if (!string.IsNullOrEmpty(paymentNotice.PayExchangeApplyID))
                {
                    CenterProductFee centerPayExchange = new CenterProductFee(this);
                    SendStrcut sendStrcut = new SendStrcut();
                    sendStrcut.sender = "FSender001";
                    sendStrcut.option = CenterConstant.Enter;
                    sendStrcut.model = centerPayExchange;
                    //提交中心
                    string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                    string requestUrl = URL + FinanceApiSetting.PaymentUrl;
                    string apiclient = JsonConvert.SerializeObject(sendStrcut);

                    Logs log = new Logs();
                    log.Name = "付汇同步";
                    log.MainID = paymentNotice.ID;
                    log.AdminID = paymentNotice.AdminID;
                    log.Json = apiclient;
                    log.Summary = "";
                    log.Enter();

                    HttpResponseMessage response = new HttpResponseMessage();
                    response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);

                    //如果有手续费，手续费也要通给中心
                    var notice = e.PaymentNotice;
                    if (notice.Poundage != null)
                    {
                        CenterFee centerFeePoundage = new CenterFee();
                        centerFeePoundage.ReceiveAccountNo = notice.BankAccount;
                        centerFeePoundage.AccountNo = notice.FinanceAccount.BankAccount;
                        centerFeePoundage.CreatorID = centerPayExchange.CreatorID;
                        if (string.IsNullOrEmpty(notice.SeqNoPoundage))
                        {
                            centerFeePoundage.SeqNo = " ";
                        }
                        else
                        {
                            centerFeePoundage.SeqNo = notice.SeqNoPoundage;
                        }
                        centerFeePoundage.Amount = notice.Poundage.Value;
                        centerFeePoundage.Currency = centerPayExchange.PayerCurrency;
                        centerFeePoundage.Rate = notice.ExchangeRate == null ? 1 : notice.ExchangeRate.Value;
                        centerFeePoundage.PaymentDate = notice.PayDate;
                        centerFeePoundage.PaymentType = (int)CenterPaymentType.BankTransfer;
                        centerFeePoundage.MoneyType = (int)MoneyTypeEnum.BankAutoApply;

                        string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(FinanceFeeType.PayBankPaypal);
                        CenterFeeItem feeItem = new CenterFeeItem();
                        feeItem.FeeType = ceterFeetype;
                        feeItem.Amount = centerFeePoundage.Amount;
                        feeItem.FeeDesc = "手续费";

                        centerFeePoundage.FeeItems = new List<CenterFeeItem>();
                        centerFeePoundage.FeeItems.Add(feeItem);

                        SendStrcut sendStrcutPoundage = new SendStrcut();
                        sendStrcutPoundage.sender = "FSender001";
                        sendStrcutPoundage.option = CenterConstant.Enter;
                        sendStrcutPoundage.model = centerFeePoundage;

                        string apiclientPoundage = JsonConvert.SerializeObject(sendStrcutPoundage);

                        Logs logP = new Logs();
                        logP.Name = "费用同步";
                        logP.MainID = paymentNotice.ID;
                        logP.AdminID = paymentNotice.AdminID;
                        logP.Json = apiclientPoundage;
                        logP.Summary = "银行手续费同步";
                        logP.Enter();

                        HttpResponseMessage responseP = new HttpResponseMessage();
                        string feerequestUrl = URL + FinanceApiSetting.FeeUrl;
                        responseP = new HttpClientHelp().HttpClient("POST", feerequestUrl, apiclientPoundage);
                    }
                }
            }
        }

        /// <summary>
        /// 调用大赢家接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaymentNotice_PaymentNotice2Dyj(object sender, PaymentNoticePaidEventArgs e)
        {
            try
            {
                //var result = new Finance.DyjFinance.DyjPayment(e.PaymentNotice);
                //result.PostToDYJ();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 完成付款后，更新手续费
        /// </summary>
        public void UpdatePoundage(decimal diff, string oldSeqPoun)
        {
            try
            {
                CenterFee centerFeePoundage = new CenterFee();
                centerFeePoundage.ReceiveAccountNo = this.BankAccount;
                centerFeePoundage.AccountNo = this.FinanceAccount.BankAccount;
                centerFeePoundage.SeqNo = this.SeqNoPoundage;
                centerFeePoundage.Amount = this.Poundage.Value;
                centerFeePoundage.Currency = this.Currency;
                centerFeePoundage.Rate = this.ExchangeRate == null ? 1 : this.ExchangeRate.Value;
                centerFeePoundage.PaymentDate = this.PayDate;
                centerFeePoundage.PaymentType = (int)CenterPaymentType.BankTransfer;
                centerFeePoundage.MoneyType = (int)MoneyTypeEnum.BankAutoApply;

                string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(FinanceFeeType.PayBankPaypal);
                CenterFeeItem feeItem = new CenterFeeItem();
                feeItem.FeeType = ceterFeetype;
                feeItem.Amount = centerFeePoundage.Amount;
                feeItem.FeeDesc = "手续费";

                centerFeePoundage.FeeItems = new List<CenterFeeItem>();
                centerFeePoundage.FeeItems.Add(feeItem);

                SendStrcut sendStrcutPoundage = new SendStrcut();
                sendStrcutPoundage.sender = "FSender001";
                sendStrcutPoundage.model = centerFeePoundage;

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNotices>(
                        new
                        {
                            Poundage = this.Poundage,
                            SeqNoPoundage = this.SeqNoPoundage,
                            UpdateDate = DateTime.Now,

                        }, item => item.ID == this.ID);


                    //如果差额就是新的手续费，说明之前没有手续费，直接新增一个
                    if (diff == this.Poundage)
                    {
                        sendStrcutPoundage.option = CenterConstant.Enter;

                        FinancePayment Poundage = new FinancePayment();
                        Poundage.SeqNo = this.SeqNoPoundage;
                        if (string.IsNullOrEmpty(this.SeqNoPoundage))
                        {
                            Poundage.SeqNo = "Pou" + this.SeqNo;
                        }
                        Poundage.PayeeName = this.BankName;
                        Poundage.Payer = this.Payer;
                        Poundage.PayFeeType = FinanceFeeType.Poundage;
                        Poundage.FinanceVault = this.FinanceVault;
                        Poundage.FinanceAccount = this.FinanceAccount;
                        Poundage.BankName = this.BankName;
                        Poundage.BankAccount = this.BankAccount;
                        Poundage.Amount = this.Poundage.Value;
                        Poundage.Currency = this.Currency;
                        Poundage.ExchangeRate = this.ExchangeRate.Value;
                        Poundage.PayType = PaymentType.TransferAccount;
                        Poundage.PayDate = DateTime.Now;
                        Poundage.Enter();
                    }
                    else
                    {
                        centerFeePoundage.OldSeqNo = oldSeqPoun;
                        sendStrcutPoundage.option = CenterConstant.Update;

                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePayments>(
                            new
                            {
                                Amount = this.Poundage,
                                SeqNo = this.SeqNoPoundage,
                                UpdateDate = DateTime.Now,

                            }, item => item.SeqNo == oldSeqPoun);



                        if (diff != 0)
                        {
                            //如果差额不是0                        
                            //还需要改流水FinanceAccountFlow
                            var financePayment = new Needs.Ccs.Services.Views.FinancePaymentView().Where(item => item.SeqNo == this.SeqNoPoundage).FirstOrDefault();
                            var sourceFlow = financePayment.FinanceAccountFlow;

                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(
                            new
                            {
                                Amount = this.Poundage,
                                SeqNo = this.SeqNoPoundage,
                                AccountBalance = sourceFlow.AccountBalance - diff,
                                UpdateDate = DateTime.Now,

                            }, item => item.ID == sourceFlow.ID);

                            DateTime createDate = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>().Where(t => t.ID == sourceFlow.ID).FirstOrDefault().CreateDate;

                            //原账户流水之后的所有账户流水的记录

                            var accountFlows = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>()
                                   .Where(item => item.CreateDate > createDate && item.FinanceAccountID == sourceFlow.FinanceAccount.ID).
                                   OrderBy(item => item.CreateDate);

                            foreach (var accountFlow in accountFlows)
                            {
                                //减去差额
                                accountFlow.AccountBalance -= diff;

                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccountFlows>(
                                new
                                {
                                    UpdateDate = DateTime.Now,
                                    AccountBalance = accountFlow.AccountBalance,
                                }, item => item.ID == accountFlow.ID);
                            }
                            //更新对应账户余额
                            var account = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>()
                                .Where(item => item.ID == sourceFlow.FinanceAccount.ID).FirstOrDefault();
                            account.Balance -= diff;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                            new
                            {
                                UpdateDate = DateTime.Now,
                                Balance = account.Balance,
                            }, item => item.ID == account.ID);

                        }
                    }
                }

                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == this.Payer.ID)?.ErmAdminID;
                centerFeePoundage.CreatorID = ErmAdminID;
                string apiclientPoundage = JsonConvert.SerializeObject(sendStrcutPoundage);
                //提交中心
                Logs logP = new Logs();
                logP.Name = "费用同步";
                logP.MainID = this.ID;
                logP.AdminID = this.Payer.ID;
                logP.Json = apiclientPoundage;
                logP.Summary = "银行手续费修改同步";
                logP.Enter();
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                HttpResponseMessage responseP = new HttpResponseMessage();
                string feerequestUrl = URL + FinanceApiSetting.FeeUrl;
                responseP = new HttpClientHelp().HttpClient("POST", feerequestUrl, apiclientPoundage);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 更新付款账户
        /// </summary>
        public void UpdateAccount()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PaymentNotices>(
                    new
                    {
                        PayeeName = this.PayeeName,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount
                    }, item => item.ID == this.ID);
            }
        }
    }
}
