using Needs.Linq;
using Needs.Utils.Descriptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单收款明细
    /// </summary>
    public class OrderReceipt : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 财务收款ID
        /// </summary>
        public string FinanceReceiptID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 费用来源
        /// </summary>
        public string FeeSourceID { get; set; }

        /// <summary>
        /// 订单收款费用类型
        /// </summary>
        public Enums.OrderFeeType FeeType { get; set; }

        /// <summary>
        /// 订单应收/实收类型
        /// </summary>
        public Enums.OrderReceiptType Type { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 收款人/跟单员
        /// </summary>
        public Admin Admin { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public Enums.OrderStatus OrderStatus { get; set; }

        public bool IsLoan { get; set; }

        #endregion

        private string _feeTypeShowName = string.Empty;

        /// <summary>
        /// 费用类型显示名称
        /// </summary>
        public string FeeTypeShowName
        {
            get
            {
                if (this.FeeType == Enums.OrderFeeType.Incidental)
                {
                    //商检费
                    if (this.FeeSourceID == null)
                    {
                        return Enums.OrderPremiumType.InspectionFee.GetDescription();
                    }
                    //其他杂费
                    using (var view = new Views.OrderPremiumsView())
                    {
                        var premium = view.Where(item => item.ID == this.FeeSourceID).FirstOrDefault();
                        if (premium.Type == Enums.OrderPremiumType.OtherFee)
                        {
                            string standardRemarkName = "";
                            if (!string.IsNullOrEmpty(premium.StandardRemark))
                            {
                                try
                                {
                                    var stdRemark = JsonConvert.DeserializeObject<StandardRemark>(premium.StandardRemark);
                                    standardRemarkName = stdRemark.SelectedStd.Name;
                                }
                                catch
                                {
                                }
                            }

                            List<string> listResult = new List<string>();
                            if (!string.IsNullOrEmpty(premium.Name))
                            {
                                listResult.Add(premium.Name);
                            }
                            if (!string.IsNullOrEmpty(standardRemarkName))
                            {
                                listResult.Add(standardRemarkName);
                            }

                            return string.Join("。", listResult.ToArray());
                        }
                        else
                        {
                            return premium.Type.GetDescription();
                        }
                    }
                }
                else
                {
                    //货款、关税、增值税、代理费
                    return this.FeeType.GetDescription();
                }
            }
            set
            {
                _feeTypeShowName = value;
            }
        }

        /// <summary>
        /// 对应的一比收款的银行流水号
        /// </summary>
        public string SeqNo { get; set; } = string.Empty;

        /// <summary>
        /// 对应的一比收款的银行收款日期
        /// </summary>
        public DateTime? ReceiptDate { get; set; }
        public bool ReImport { get; set; }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public OrderReceipt()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.ReImport = false;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        virtual public void Enter()
        {
            //业务逻辑在子类中实现

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        public void OrderReceiptEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {//&& item.Type == (int)Enums.OrderReceiptType.Received && item.FeeType == (int)Enums.OrderFeeType.Product
                var orderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(item => item.FeeSourceID == FeeSourceID 
                                                                                                                && item.OrderID == OrderID 
                                                                                                                && item.Type == (int)Enums.OrderReceiptType.Received 
                                                                                                                && item.FeeType == (int)Enums.OrderFeeType.Product 
                                                                                                                && item.Status == (int)Enums.Status.Normal).ToList();
                if (orderReceipts.Count != 0)
                {
                    foreach (var orderReceipt in orderReceipts)
                    {
                        Amount = orderReceipt.Amount;
                        ID = orderReceipt.ID;
                        FinanceReceiptID = orderReceipt.FinanceReceiptID;
                        if (Amount != 0)
                        {
                            var receiptNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>().FirstOrDefault(item => item.ID == FinanceReceiptID);
                            if (receiptNotice != null)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new
                                {
                                    ClearAmount = receiptNotice.ClearAmount + Amount
                                }, item => item.ID == FinanceReceiptID);

                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new
                                {
                                    Amount = Amount - Amount
                                }, item => item.ID == ID);

                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }

    /// <summary>
    /// 订单应收
    /// </summary>
    public class OrderReceivable : OrderReceipt
    {
        public OrderReceivable() : base()
        {

        }

        //应收关税/增值税/代理费/商检费
        public OrderReceivable(Order order, Enums.OrderFeeType type) : this()
        {
            this.ClientID = order.Client.ID;
            this.OrderID = order.ID;
            this.FeeType = type;
            this.Type = Enums.OrderReceiptType.Receivable;
            this.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
            this.Rate = 1M;
            this.Admin = order.Client.Merchandiser;
        }

        //应收杂费(非商检费)
        public OrderReceivable(Order order, OrderPremium premium) : this()
        {
            this.ClientID = order.Client.ID;
            this.OrderID = premium.OrderID;
            this.FeeSourceID = premium.ID;
            this.FeeType = Enums.OrderFeeType.Incidental;
            this.Type = Enums.OrderReceiptType.Receivable;
            this.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
            this.Rate = 1M;
            this.Amount = ((premium.UnitPrice * premium.Count * premium.Rate) * (1 + order.ClientAgreement.InvoiceTaxRate)).ToRound(2);
            this.Admin = premium.Admin;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    //主键ID（OrderReceipt +8位年月日+6位流水号）
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            base.Enter();
        }
    }

    /// <summary>
    /// 订单实收
    /// </summary>
    public class OrderReceived : OrderReceipt
    {

        public string DyjID { get; set; }

        public OrderReceived() : base()
        {
            this.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
            this.Rate = 1;
            this.Type = Enums.OrderReceiptType.Received;

            this.EnterSuccess += OrderReceived_EnterSuccess;
        }

        //public OrderReceived(string ID) : this()
        //{
        //    this.ID = ID;
        //}

        public OrderReceived(OrderReceipt orderReceipt, Enums.Status status) : this()
        {
            this.ID = orderReceipt.ID;
            this.FinanceReceiptID = orderReceipt.FinanceReceiptID;
            this.ClientID = orderReceipt.ClientID;
            this.OrderID = orderReceipt.OrderID;
            this.FeeSourceID = orderReceipt.FeeSourceID;
            this.FeeType = orderReceipt.FeeType;
            this.Type = orderReceipt.Type;
            this.Currency = orderReceipt.Currency;
            this.Rate = orderReceipt.Rate;
            //实收金额在订单收款表中存负值
            this.Amount = 0 - orderReceipt.Amount;
            this.Admin = orderReceipt.Admin;

            this.Status = status;

            this.CreateDate = orderReceipt.CreateDate;
            this.UpdateDate = DateTime.Now;
            this.Summary = orderReceipt.Summary;
        }

        /// <summary>
        /// 收款通知ID（跟单基于收款通知维护订单实收）
        /// </summary>
        public string ReceiptNoticeID { get; set; }

        public string SeqNo { get; set; }

        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    //主键ID（OrderReceipt +8位年月日+6位流水号）
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            base.Enter();
        }

        private void OrderReceived_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var received = (OrderReceived)e.Object;

            if (received.Status == Enums.Status.Normal)
            {
                var noticeID = received.ReceiptNoticeID;
                //收款通知的已明确金额
                var clearAmount = new Views.ReceiptNoticesView()[noticeID].ClearAmount;

                //更新收款通知的已明确金额
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new { ClearAmount = clearAmount + received.Amount }, item => item.ID == noticeID);
                }

                //插入 SwapNoticeReceiptUses 表 未处理数据 Begin

                Task.Run(() =>
                {
                    string newSwapNoticeReceiptUseID = Guid.NewGuid().ToString("N");

                    SwapNoticeReceiptUse swapNoticeReceiptUse = new SwapNoticeReceiptUse();
                    swapNoticeReceiptUse.ID = newSwapNoticeReceiptUseID;
                    swapNoticeReceiptUse.Type = Enums.ReceiptUseType.UnHandle;
                    swapNoticeReceiptUse.OrderReceiptID = received.ID;
                    swapNoticeReceiptUse.OrderReceiptAmount = received.Amount;
                    swapNoticeReceiptUse.Status = Enums.Status.Normal;
                    swapNoticeReceiptUse.CreateDate = DateTime.Now;
                    swapNoticeReceiptUse.UpdateDate = DateTime.Now;
                    swapNoticeReceiptUse.Enter();

                    SwapNoticeReceiptUseLog swapNoticeReceiptUseLog = new SwapNoticeReceiptUseLog();
                    swapNoticeReceiptUseLog.ID = Guid.NewGuid().ToString("N");
                    swapNoticeReceiptUseLog.SwapNoticeReceiptUseID = newSwapNoticeReceiptUseID;
                    swapNoticeReceiptUseLog.OrderReceiptID = swapNoticeReceiptUse.OrderReceiptID;
                    swapNoticeReceiptUseLog.OrderReceiptAmount = swapNoticeReceiptUse.OrderReceiptAmount;
                    swapNoticeReceiptUseLog.Status = Enums.Status.Normal;
                    swapNoticeReceiptUseLog.CreateDate = DateTime.Now;
                    swapNoticeReceiptUseLog.UpdateDate = DateTime.Now;
                    swapNoticeReceiptUseLog.Summary = "新增未处理";
                    swapNoticeReceiptUseLog.Enter();
                });

                //插入 SwapNoticeReceiptUses 表 未处理数据 End
            }
        }
    }

    /// <summary>
    /// 用于批量插入的 OrderReceived
    /// </summary>
    public class OrderReceivedBatch
    {
        private List<OrderReceived> List { get; set; }

        public OrderReceivedBatch(List<OrderReceived> list)
        {
            this.List = list;
        }

        public void Insert()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderReceipts = this.List.Select(entity => new Layer.Data.Sqls.ScCustoms.OrderReceipts
                {
                    ID = entity.ID,
                    FinanceReceiptID = entity.ReceiptNoticeID,
                    ClientID = entity.ClientID,
                    OrderID = entity.OrderID,
                    FeeSourceID = entity.FeeSourceID,
                    FeeType = (int)entity.FeeType,
                    Type = (int)entity.Type,
                    Currency = entity.Currency,
                    Rate = entity.Rate,
                    //实收金额在订单收款表中存负值
                    Amount = -entity.Amount,
                    AdminID = entity.Admin.OriginID,
                    Status = (int)entity.Status,
                    CreateDate = entity.CreateDate,
                    UpdateDate = DateTime.Now,
                    Summary = entity.Summary
                }).ToArray();

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderReceipts>(orderReceipts);


                //更新 ReceiptNotices
                string receiptNoticeID = "";
                decimal addClearAmount = 0;
                if (this.List != null && this.List.Any())
                {
                    foreach (var item in this.List)
                    {
                        if (!string.IsNullOrEmpty(item.ReceiptNoticeID))
                        {
                            receiptNoticeID = item.ReceiptNoticeID;
                        }

                        addClearAmount += item.Amount;
                    }
                }



                //收款通知的已明确金额
                var clearAmount = new Views.ReceiptNoticesView()[receiptNoticeID].ClearAmount;

                //更新收款通知的已明确金额
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new { ClearAmount = clearAmount + addClearAmount }, item => item.ID == receiptNoticeID);

            }

            //插入 SwapNoticeReceiptUses 表 未处理数据 Begin

            Task.Run(() =>
            {
                if (this.List != null && this.List.Any())
                {
                    foreach (var item in this.List)
                    {
                        string newSwapNoticeReceiptUseID = Guid.NewGuid().ToString("N");

                        SwapNoticeReceiptUse swapNoticeReceiptUse = new SwapNoticeReceiptUse();
                        swapNoticeReceiptUse.ID = newSwapNoticeReceiptUseID;
                        swapNoticeReceiptUse.Type = Enums.ReceiptUseType.UnHandle;
                        swapNoticeReceiptUse.OrderReceiptID = item.ID;
                        swapNoticeReceiptUse.OrderReceiptAmount = item.Amount;
                        swapNoticeReceiptUse.Status = Enums.Status.Normal;
                        swapNoticeReceiptUse.CreateDate = DateTime.Now;
                        swapNoticeReceiptUse.UpdateDate = DateTime.Now;
                        swapNoticeReceiptUse.Enter();

                        SwapNoticeReceiptUseLog swapNoticeReceiptUseLog = new SwapNoticeReceiptUseLog();
                        swapNoticeReceiptUseLog.ID = Guid.NewGuid().ToString("N");
                        swapNoticeReceiptUseLog.SwapNoticeReceiptUseID = newSwapNoticeReceiptUseID;
                        swapNoticeReceiptUseLog.OrderReceiptID = swapNoticeReceiptUse.OrderReceiptID;
                        swapNoticeReceiptUseLog.OrderReceiptAmount = swapNoticeReceiptUse.OrderReceiptAmount;
                        swapNoticeReceiptUseLog.Status = Enums.Status.Normal;
                        swapNoticeReceiptUseLog.CreateDate = DateTime.Now;
                        swapNoticeReceiptUseLog.UpdateDate = DateTime.Now;
                        swapNoticeReceiptUseLog.Summary = "新增未处理";
                        swapNoticeReceiptUseLog.Enter();
                    }
                }
            });

            //插入 SwapNoticeReceiptUses 表 未处理数据 End

        }
    }

    /// <summary>
    /// 退款实收
    /// </summary>
    public class UnmarkOrderReceipt : OrderReceipt
    {
        public UnmarkOrderReceipt() : base()
        {
            this.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
            this.Rate = 1;
            this.Type = Enums.OrderReceiptType.Received;
            this.Status = Enums.Status.Delete;

            this.EnterSuccess += UnmarkOrderReceipt_EnterSuccess;
        }

        /// <summary>
        /// 收款通知ID（跟单基于收款通知维护订单实收）
        /// </summary>
        public string ReceiptNoticeID { get; set; }

        public string SeqNo { get; set; }

        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());

                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            base.Enter();
        }

        private void UnmarkOrderReceipt_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var unmarkReceived = (UnmarkOrderReceipt)e.Object;
            var noticeID = unmarkReceived.ReceiptNoticeID;
            //收款通知的已明确金额
            var clearAmount = new Views.ReceiptNoticesView()[noticeID].ClearAmount;

            //更新收款通知的已明确金额
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new { ClearAmount = clearAmount + unmarkReceived.Amount }, item => item.ID == noticeID);
            }
        }
    }
}
