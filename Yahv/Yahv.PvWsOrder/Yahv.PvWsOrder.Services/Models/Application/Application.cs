using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 收付款申请
    /// </summary>
    public class Application : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ClientID { get; set; }

        public ApplicationType Type { get; set; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 对美元的汇率
        /// </summary>
        public decimal? RateToUSD { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public ApplicationStatus ApplicationStatus { get; set; }

        /// <summary>
        /// 收款状态
        /// </summary>
        public ApplicationReceiveStatus ReceiveStatus { get; set; }

        /// <summary>
        /// 付款状态
        /// </summary>
        public ApplicationPaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsEntry { get; set; }

        /// <summary>
        /// 发货时机
        /// </summary>
        public DelivaryOpportunity? DelivaryOpportunity { get; set; }

        /// <summary>
        /// 发票投递方式
        /// </summary>
        public CheckDeliveryType? CheckDelivery { get; set; }

        /// <summary>
        /// 送票承运商
        /// </summary>
        public string CheckCarrier { get; set; }

        /// <summary>
        /// 送票运单号
        /// </summary>
        public string CheckWaybillCode { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 支票导入账户
        /// </summary>
        public string CheckPayeeAccount { get; set; }

        /// <summary>
        /// 发票收件人地址
        /// </summary>
        public string CheckConsignee { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string CheckTitle { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 收款核销日期
        /// </summary>
        public DateTime? ReceiveDate { get; set; }

        /// <summary>
        /// 付款核销日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 我方收款账户
        /// </summary>
        public string InCompanyName { get; set; }

        /// <summary>
        /// 我方收款账号
        /// </summary>
        public string InBankAccount { get; set; }

        /// <summary>
        /// 我方收款银行
        /// </summary>
        public string InBankName { get; set; }

        /// <summary>
        /// 我方银行地址
        /// </summary>
        public string InBankAddress { get; set; }

        /// <summary>
        /// 我方银行编码
        /// </summary>
        public string InSwiftCode { get; set; }

        /// <summary>
        /// 我方付款账户
        /// </summary>
        public string OutCompanyName { get; set; }

        /// <summary>
        /// 我方付款银行
        /// </summary>
        public string OutBankName { get; set; }

        /// <summary>
        /// 我方付款账号
        /// </summary>
        public string OutBankAccount { get; set; }


        /// <summary>
        /// 代付款手续费类型
        /// </summary>
        public string HandlingFeePayerType { get; set; }

        /// <summary>
        /// 手续费（美元）
        /// </summary>
        public decimal? HandlingFee { get; set; }

        /// <summary>
        /// 美元实时汇率
        /// </summary>
        public decimal? USDRate { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 申请客户
        /// </summary>
        public WsClient Client { get; set; }

        /// <summary>
        /// 客户付款人
        /// </summary>
        public IEnumerable<ApplicationPayer> Payers { get; set; }

        /// <summary>
        /// 客户收款人
        /// </summary>
        public IEnumerable<ApplicationPayee> Payees { get; set; }

        /// <summary>
        /// 申请订单项明细
        /// </summary>
        public IEnumerable<ApplicationItem> Items { get; set; }

        /// <summary>
        /// 收付款申请的应收记录
        /// </summary>
        public IEnumerable<VoucherStatistic> VouchersStatistic
        {
            get
            {
                return new Views.Alls.ApplicationVouchersStatisticsRoll(this.ID);
            }
        }

        /// <summary>
        /// 申请附件
        /// </summary>
        IEnumerable<CenterFileDescription> fileItems;
        public IEnumerable<CenterFileDescription> FileItems
        {
            get
            {
                if (this.fileItems == null)
                {
                    this.fileItems = new Views.ApplicationFilesRoll(this.ID);
                }
                return this.fileItems;
            }
            set
            {
                this.fileItems = value;
            }
        }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        public void OnAbandonSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
        private void Order_EnterSuccess(object sender, SuccessEventArgs e)
        {

        }
        private void Order_AbandonSuccess(object sender, SuccessEventArgs e)
        {

        }

        #endregion

        public Application()
        {
            this.Status = GeneralStatus.Normal;
            this.ApplicationStatus = ApplicationStatus.Examining;
            this.ReceiveStatus = ApplicationReceiveStatus.UnReceive;
            this.PaymentStatus = ApplicationPaymentStatus.UnPay;
            this.CreateDate = DateTime.Now;
            this.IsEntry = true;
        }

        #region 持久化

        public void Enter()
        {
            int? handlingFeePayerType = null;
            if (int.TryParse(this.HandlingFeePayerType, out var handlingFeePayerType_1))
            {
                handlingFeePayerType = handlingFeePayerType_1;
            }

            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>().Any(item => item.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Application);
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Applications
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        Type = (int)this.Type,
                        TotalPrice = this.TotalPrice,
                        Currency = (int)this.Currency,
                        InCompanyName = this.InCompanyName,
                        InBankName = this.InBankName,
                        InBankAccount = this.InBankAccount,
                        OutCompanyName = this.OutCompanyName,
                        OutBankName = this.OutBankName,
                        OutBankAccount = this.OutBankAccount,
                        Status = (int)this.Status,
                        ApplicationStatus = (int)this.ApplicationStatus,
                        ReceiveStatus = (int)this.ReceiveStatus,
                        PaymentStatus = (int)this.PaymentStatus,
                        IsEntry = this.IsEntry,
                        DelivaryOpportunity = (int)this.DelivaryOpportunity.GetValueOrDefault(),
                        CheckCarrier = this.CheckCarrier,
                        CheckDelivery = (int)this.CheckDelivery.GetValueOrDefault(),
                        CheckWaybillCode = this.CheckWaybillCode,
                        CreateDate = this.CreateDate,
                        PaymentDate = this.PaymentDate,
                        ReceiveDate = this.ReceiveDate,
                        UserID = this.UserID,
                        CheckPayeeAccount = this.CheckPayeeAccount,
                        CheckConsignee = this.CheckConsignee,
                        CheckTitle = this.CheckTitle,
                        HandlingFeePayerType = handlingFeePayerType,
                        HandlingFee = this.HandlingFee,
                        USDRate = this.USDRate,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                    {
                        ClientID = this.ClientID,
                        Type = (int)this.Type,
                        TotalPrice = this.TotalPrice,
                        Currency = (int)this.Currency,
                        InCompanyName = this.InCompanyName,
                        InBankName = this.InBankName,
                        InBankAccount = this.InBankAccount,
                        OutCompanyName = this.OutCompanyName,
                        OutBankName = this.OutBankName,
                        OutBankAccount = this.OutBankAccount,
                        Status = (int)this.Status,
                        ApplicationStatus = (int)this.ApplicationStatus,
                        ReceiveStatus = (int)this.ReceiveStatus,
                        PaymentStatus = (int)this.PaymentStatus,
                        IsEntry = this.IsEntry,
                        DelivaryOpportunity = (int)this.DelivaryOpportunity.GetValueOrDefault(),
                        CheckCarrier = this.CheckCarrier,
                        CheckDelivery = (int)this.CheckDelivery.GetValueOrDefault(),
                        CheckWaybillCode = this.CheckWaybillCode,
                        CreateDate = this.CreateDate,
                        PaymentDate = this.PaymentDate,
                        ReceiveDate = this.ReceiveDate,
                        UserID = this.UserID,
                        CheckPayeeAccount = this.CheckPayeeAccount,
                        CheckConsignee = this.CheckConsignee,
                        CheckTitle = this.CheckTitle,
                        HandlingFeePayerType = handlingFeePayerType,
                        HandlingFee = this.HandlingFee,
                        USDRate = this.USDRate,
                    }, item => item.ID == this.ID);
                }
            };
            this.OnEnterSuccess();
        }

        public void Abandon()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                {
                    Status = GeneralStatus.Closed,
                }, item => item.ID == this.ID);

                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ApplicationItems>(new
                {
                    Status = GeneralStatus.Closed,
                }, item => item.ApplicationID == this.ID);

                this.OnAbandonSuccess();
            }
        }

        //审核
        public void Examine(bool flag, Application_Logs log)
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                if (flag)
                {
                    //获取实时汇率
                    var rate = Yahv.Payments.ExchangeRates.Floating[this.Currency, Currency.USD];
                    //审核通过
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                    {
                        RateToUSD = rate,
                        ApplicationStatus = ApplicationStatus.Examined,
                    }, item => item.ID == this.ID);
                }
                else
                {
                    //审核驳回
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                    {
                        ApplicationStatus = ApplicationStatus.Reject,
                    }, item => item.ID == this.ID);

                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ApplicationItems>(new
                    {
                        Status = GeneralStatus.Closed,
                    }, item => item.ApplicationID == this.ID);
                }
                //保存日志
                log.Enter();
            }
        }

        //审批
        public void Approve(bool flag, Application_Logs log)
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                if (flag)
                {
                    //审批通过
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                    {
                        ApplicationStatus = ApplicationStatus.Approved,
                    }, item => item.ID == this.ID);
                }
                else
                {
                    //审批驳回
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                    {
                        ApplicationStatus = ApplicationStatus.Reject,
                        ReceiveStatus = ApplicationReceiveStatus.UnReceive,
                        PaymentStatus = ApplicationPaymentStatus.UnPay,
                    }, item => item.ID == this.ID);

                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ApplicationItems>(new
                    {
                        Status = GeneralStatus.Closed,
                    }, item => item.ApplicationID == this.ID);
                }
                //保存日志
                log.Enter();
            }
        }

        //收款核销完成
        public void ReveiveWorkOff()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                {
                    ReceiveDate = DateTime.Now,
                    ReceiveStatus = ApplicationReceiveStatus.Received,
                }, item => item.ID == this.ID);
            }
        }

        //付款核销完成
        public void PaymentWorkOff()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                {
                    PaymentDate = DateTime.Now,
                    PaymentStatus = ApplicationPaymentStatus.Paid,
                }, item => item.ID == this.ID);
            }
        }
        #endregion

        /// <summary>
        /// 保存订单文件
        /// </summary>
        /// <param name="newFiles"></param>
        /// <param name="oldFiles"></param>
        private void SaveApplicationFiles(IEnumerable<CenterFileDescription> newFiles, IEnumerable<CenterFileDescription> oldFiles)
        {
            string[] newids = newFiles.Select(item => item.ID).ToArray();
            string[] oldids = oldFiles.Select(item => item.ID).ToArray();
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //删除原文件
                foreach (var id in oldids)
                {
                    if (!newids.Contains(id))
                    {
                        //删除原来的项
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            ApplicationID = "",
                        }, item => item.ID == id && item.ApplicationID == this.ID);
                    }
                }
                //订单绑定新文件
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    ApplicationID = this.ID,
                }, item => newids.Contains(item.ID));
            }
        }
    }
}
