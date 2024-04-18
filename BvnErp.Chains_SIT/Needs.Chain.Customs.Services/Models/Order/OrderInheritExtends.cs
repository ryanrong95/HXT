using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;
using System.Transactions;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 草稿订单
    /// </summary>
    public class DraftOrder : Order
    {
        /// <summary>
        /// 当订单删除时发生
        /// </summary>
        public event OrderAbandonHanlder Deleted;

        /// <summary>
        /// 当客户确认下单时发生
        /// </summary>
        public event StatusChangedEventHanlder Confirmed;

        public DraftOrder()
        {
            this.Deleted += Order_Deleted;
            this.Confirmed += Order_Confirmed;
        }

        private void Order_Deleted(object sender, OrderAbandonEventArgs e)
        {
            // 写入日志
            if (e.Order.Admin != null)
            {
                e.Order.Log(e.Order.Admin, "跟单员[" + e.Order.Admin.RealName + "]删除了订单");
            }

            if (e.Order.User != null)
            {
                e.Order.Log(e.Order.User, "用户[" + e.Order.User.RealName + "]删除了订单");
            }
        }

        private void Order_Confirmed(object sender, StatusChangedEventArgs e)
        {
            var order = (DraftOrder)e.Object;
            //写入日志
            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]确认下单，等待产品归类。");
            }
            if (order.User != null)
            {
                order.Log(order.User, "用户[" + order.User.RealName + "]确认下单，等待产品归类。");
            }
        }

        void OnDeleted()
        {
            if (this != null && this.Deleted != null)
            {
                this.Deleted(this, new OrderAbandonEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            this.Status = Enums.Status.Delete;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnDeleted();
        }

        void OnConfirmed()
        {
            if (this != null && this.Confirmed != null)
            {
                this.Confirmed(this, new StatusChangedEventArgs(this));
            }
        }

        /// <summary>
        /// 确认下单/提交订单
        /// </summary>
        public void Confirm()
        {
            this.OrderStatus = Enums.OrderStatus.Confirmed;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.Confirmed }, item => item.ID == this.ID);
            }

            this.OnConfirmed();
        }
    }

    /// <summary>
    /// 待归类订单
    /// </summary>
    public class UnClassifyOrder : Order
    {
        public UnClassifyOrder()
        {
        }
    }


    /// <summary>
    /// 归类完成，等待报价的订单
    /// </summary>
    public class ClassifiedOrder : Order
    {
        /// <summary>
        /// 当订单报价时发生
        /// </summary>
        public event OrderQuotedHanlder Quoted;

        public ClassifiedOrder()
        {
            this.Quoted += Order_Quoted;
            //this.Quoted += Order_QuotedNotice;
        }

        private void Order_Quoted(object sender, OrderQuotedEventArgs e)
        {          
            //写入日志
            if (e.Order.Admin != null)
            {
                e.Order.Log(e.Order.Admin, "跟单员[" + e.Order.Admin.RealName + "]完成了订单报价，等待客户确认");               
                e.Order.Trace(e.Order.Admin, OrderTraceStep.Processing, "您的订单已经报价。请您核对报价信息，并确认报价");
            }
            if (e.Order.APIAdmin != null)
            {
                e.Order.Log(e.Order.APIAdmin, "[" + e.Order.APIAdmin.RealName + "]通过接口完成了订单报价");               
                e.Order.Trace(e.Order.APIAdmin, OrderTraceStep.Processing, "您的订单已经报价。请您核对报价信息，并确认报价");                               
            }

            //调用代仓储前端接口
            var confirm = new ClientConfirm();
            confirm.OrderID = e.Order.MainOrderID;
            confirm.AdminID = e.Order.Admin.ID;
            confirm.Type = ConfirmType.Normal;

            var apisetting = new ApiSettings.PvWsOrderApiSetting();
            var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ClientConfirm;
            var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, confirm);
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);
            if (message.code != 200)
            {
                e.Order.Log(e.Order.Admin, "订单报价推送代仓储失败:" + confirm.OrderID + " " + confirm.Type.GetDescription() + " " + message.data);                
            }

            //发送消息
            System.Threading.Tasks.Task.Run(() =>
            {
                PushMsg pushMsg = new PushMsg((int)SpotName.Classified, e.Order.ID);
                pushMsg.push();
            });
        }

        private void Order_QuotedNotice(object sender, OrderQuotedEventArgs e)
        {
            NoticeLog noticeLog = new NoticeLog();
            noticeLog.MainID = e.Order.ID;
            noticeLog.NoticeType = SendNoticeType.ClassifyDone;
            noticeLog.Readed = true;
            noticeLog.SendNotice();
        }

        public void OnQuoted()
        {
            if (this != null && this.Quoted != null)
            {
                this.Quoted(this, new OrderQuotedEventArgs(this));
            }
        }

        /// <summary>
        /// 订单报价
        /// </summary>
        public void Quote()
        {
            this.OrderStatus = Enums.OrderStatus.Quoted;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新订单状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.Quoted }, item => item.ID == this.ID);
            }

            this.OnQuoted();
        }
    }

    /// <summary>
    /// 报价完成，等待客户确认的订单
    /// </summary>
    public class QuotedOrder : Order
    {
        /// <summary>
        /// 订单取消原因
        /// </summary>
        public string CanceledSummary { get; set; }

        /// <summary>
        /// 当客户确认订单报价时发生
        /// </summary>
        public event OrderQuoteConfirmedHanlder QuoteConfirmed;

        /// <summary>
        /// 当订单取消时发生
        /// </summary>
        public event OrderCanceledHanlder Canceled;

        public QuotedOrder()
        {
            this.Canceled += Order_Canceled;
            this.QuoteConfirmed += Order_QuoteConfirmed;
        }

        private void Order_Canceled(object sender, Hanlders.OrderCanceledEventArgs e)
        {
            var order = (QuotedOrder)e.Order;
            //写入日志
            if (order.User != null)
            {
                order.Log(order.User, "用户[" + order.User.RealName + "]取消了订单,订单取消原因：" + order.CanceledSummary);
            }
        }

        private void Order_QuoteConfirmed(object sender, OrderQuoteConfirmedEventArgs e)
        {

            //写入日志
            if (e.Order.User != null)
            {
                e.Order.Trace(e.Order.User, OrderTraceStep.Processing, "您的订单已由本人确认");                
                e.Order.Log(e.Order.User, "用户[" + e.Order.User.RealName + "]确认了订单报价，等待报关");
                
            }
            if (e.Order.APIAdmin != null)
            {            
                e.Order.Trace(e.Order.User, OrderTraceStep.Processing, "您的订单已由本人确认");            
                e.Order.Log(e.Order.APIAdmin, "[" + e.Order.APIAdmin.RealName + "]通过接口确认了订单报价");
               
            }
            Task.Run(() =>
            {
                //将应收关税、增值税、代理费、商检费写入订单收款(OrderReceipts)表
                e.Order.ToReceivables();
                //生成香港仓库的入库通知/提货通知
                e.Order.ToEntryNotice();
                //icgoo.IcgooToEntryNoticeSpeed(e.Order.OrderConsignee.Type == HKDeliveryType.PickUp);

                #region 超垫款上限管控

                //如果客户未付款/欠款超出协议里的垫款上限，则将订单挂起
                //垫款上限
                var productFeeLimit = e.Order.ClientAgreement.ProductFeeClause.UpperLimit.GetValueOrDefault();
                var taxFeeLimit = e.Order.ClientAgreement.TaxFeeClause.UpperLimit.GetValueOrDefault();
                var agencyFeeLimit = e.Order.ClientAgreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();
                var incidentalFeeLimit = e.Order.ClientAgreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();
                //客户的未付款/欠款
                var unpaidFees = new Views.OrderReceiptsAllsView().Where(item => item.ClientID == e.Order.Client.ID).ToList();
                var unpaidProductFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Product && item.IsLoan)
                                             .Sum(item => item.Amount * item.Rate);
                var unpaidTaxFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Tariff || item.FeeType == OrderFeeType.ExciseTax || item.FeeType == OrderFeeType.AddedValueTax)
                                             .Sum(item => item.Amount * item.Rate);
                var unpaidAgencyFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);
                var unpaidIncidentalFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);
                if (unpaidProductFee > productFeeLimit || unpaidTaxFee > taxFeeLimit || unpaidAgencyFee > agencyFeeLimit || unpaidIncidentalFee > incidentalFeeLimit)
                {
                    //生成订单管控
                    e.Order.HangUp(Enums.OrderControlType.ExceedLimit);
                }

                #endregion          
            });
        }

        virtual protected void OnCanceled()
        {
            if (this != null && this.Canceled != null)
            {
                this.Canceled(this, new OrderCanceledEventArgs(this));
            }
        }

        /// <summary>
        /// 订单取消
        /// </summary>
        public void Cancel()
        {
            this.OrderStatus = Enums.OrderStatus.Canceled;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.Canceled }, item => item.ID == this.ID);
            }

            this.OnCanceled();
        }

        void OnQuoteConfirmed()
        {
            if (this != null && this.QuoteConfirmed != null)
            {
                this.QuoteConfirmed(this, new OrderQuoteConfirmedEventArgs(this));
            }
        }

        /// <summary>
        /// 客户确认报价
        /// </summary>
        public void QuoteConfirm()
        {
            this.OrderStatus = Enums.OrderStatus.QuoteConfirmed;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.QuoteConfirmed }, item => item.ID == this.ID);
            }
            this.OnQuoteConfirmed();
        }

        /// <summary>
        /// Icgoo接口下单，有一个型号没归类时用，因为这个时候已经生成了香港仓库的入库通知，装箱，封箱了，只要改个订单状态
        /// </summary>
        public void IcgooQuoteConfirm()
        {
            this.OrderStatus = Enums.OrderStatus.QuoteConfirmed;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.QuoteConfirmed }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 修改型号信息（删除型号/修改数量）后确定订单
        /// </summary>
        public void ModelModifiedConfirm()
        {
            //处理该订单所有 删除型号/修改数量 的管控
            ClientUnConfirmedControl clientUnConfirmedControl = new ClientUnConfirmedControl(this.ID, this.OrderStatus.GetHashCode(), this.User);
            clientUnConfirmedControl.CancelHangUp();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                      

                //记录日志
                if (this.User != null)
                {                                       
                    this.Trace(this.User, OrderTraceStep.Processing, "您的订单已由本人确认（订单信息变更）");                    
                    this.Log(this.User, "用户[" + this.User.RealName + "]确认了订单报价（订单信息变更）");                   
                }


            }


            //判断是否超垫款上限管控
            Task.Run(() =>
                {
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                    {
                        //将原先 超垫款上限管控 相关信息都删除
                        var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                        var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                        var unAuditedExceedLimitControls = (from orderControl in orderControls
                                                            join orderControlStep in orderControlSteps
                                                                  on new
                                                                  {
                                                                      OrderControlID = orderControl.ID,
                                                                      OrderControlStatus = orderControl.Status,
                                                                      OrderControlStepStatus = (int)Enums.Status.Normal,
                                                                      OrderID = orderControl.OrderID,
                                                                      ControlType = orderControl.ControlType,
                                                                      ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                                                  }
                                                                  equals new
                                                                  {
                                                                      OrderControlID = orderControlStep.OrderControlID,
                                                                      OrderControlStatus = (int)Enums.Status.Normal,
                                                                      OrderControlStepStatus = orderControlStep.Status,
                                                                      OrderID = this.ID,
                                                                      ControlType = (int)Enums.OrderControlType.ExceedLimit,
                                                                      ControlStatus = orderControlStep.ControlStatus,
                                                                  }
                                                            select new OrderControlData
                                                            {
                                                                ID = orderControl.ID,
                                                            }).ToList();

                        if (unAuditedExceedLimitControls != null && unAuditedExceedLimitControls.Any())
                        {
                            string[] unAuditedExceedLimitControlIDs = unAuditedExceedLimitControls.Select(t => t.ID).ToArray();

                            foreach (var id in unAuditedExceedLimitControlIDs)
                            {
                                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderControls>(item => item.ID == id);
                                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(item => item.OrderControlID == id);
                            }
                        }

                        reponsitory.Submit();
                    }

                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //如果客户未付款/欠款超出协议里的垫款上限，则生成新的 超垫款上限管控
                        //垫款上限
                        var productFeeLimit = this.ClientAgreement.ProductFeeClause.UpperLimit.GetValueOrDefault();
                        var taxFeeLimit = this.ClientAgreement.TaxFeeClause.UpperLimit.GetValueOrDefault();
                        var agencyFeeLimit = this.ClientAgreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();
                        var incidentalFeeLimit = this.ClientAgreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();
                        //客户的未付款/欠款
                        var unpaidFees = new Views.OrderReceiptsAllsView().Where(item => item.ClientID == this.Client.ID).ToList();
                        var unpaidProductFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Product && item.IsLoan)
                                                     .Sum(item => item.Amount * item.Rate);
                        var unpaidTaxFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Tariff || item.FeeType == OrderFeeType.AddedValueTax)
                                                     .Sum(item => item.Amount * item.Rate);
                        var unpaidAgencyFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);
                        var unpaidIncidentalFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);
                        if (unpaidProductFee > productFeeLimit || unpaidTaxFee > taxFeeLimit || unpaidAgencyFee > agencyFeeLimit || unpaidIncidentalFee > incidentalFeeLimit)
                        {
                            //生成订单管控
                            this.HangUp(Enums.OrderControlType.ExceedLimit);
                        }

                        var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                        var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                        //通过当前 control、controlStep 信息判断 订单的 IsHangUp 应该是 True/False
                        int unAuditedControlsCount = (from orderControl in orderControls
                                                      join orderControlStep in orderControlSteps
                                                            on new
                                                            {
                                                                OrderControlID = orderControl.ID,
                                                                OrderControlStatus = orderControl.Status,
                                                                OrderControlStepStatus = (int)Enums.Status.Normal,
                                                                OrderID = orderControl.OrderID,

                                                                ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                                            }
                                                            equals new
                                                            {
                                                                OrderControlID = orderControlStep.OrderControlID,
                                                                OrderControlStatus = (int)Enums.Status.Normal,
                                                                OrderControlStepStatus = orderControlStep.Status,
                                                                OrderID = this.ID,

                                                                ControlStatus = orderControlStep.ControlStatus,
                                                            }
                                                      select new OrderControlData
                                                      {
                                                          ID = orderControl.ID,
                                                      }).Count();

                        if (unAuditedControlsCount <= 0)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                            { IsHangUp = false }, item => item.ID == this.ID);
                        }
                        else
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                            { IsHangUp = true }, item => item.ID == this.ID);
                        }

                    }
                });




        }
    }

    /// <summary>
    /// 客户确认报价，待报关的订单
    /// </summary>
    public class QuoteConfirmedOrder : Order
    {
        /// <summary>
        /// 当订单报关完成时发生
        /// </summary>
        public event OrderDeclareSucceedHanlder DeclareSucceed;

        public QuoteConfirmedOrder()
        {
            this.DeclareSucceed += Order_DeclareSucceed;
        }

        /// <summary>
        /// 订单-报关单报关成功
        /// </summary>
        public void DeclareSuccess()
        {
            using (var view = new Views.DecHeadsView())
            {
                #region 不判断报关数量，直接修改订单状态
                ////报关完成数量
                //var decQty = 0M;
                //var heads = view.Where(head => head.OrderID == this.ID).ToList();
                //heads.ForEach(head =>
                //{
                //    if (head.IsDeclareSuccess)
                //    {
                //        decQty += head.Lists.Sum(list => list.GQty);
                //    }
                //});

                ////全部报关完成
                //if (this.Items.Sum(item => item.Quantity) == decQty)
                //{
                //    //修改订单状态
                //    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                //    {
                //        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = OrderStatus.Declared }, item => item.ID == this.ID);
                //    }
                //    this.OnDeclareSucceed();
                //}
                #endregion

                //判断待报关状态
                if (this.OrderStatus == OrderStatus.QuoteConfirmed)
                {
                    //修改订单状态
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = OrderStatus.Declared }, item => item.ID == this.ID);
                    }
                    this.OnDeclareSucceed();
                }

            }
        }

        virtual protected void OnDeclareSucceed()
        {
            if (this != null && this.DeclareSucceed != null)
            {
                this.DeclareSucceed(this, new OrderDeclareSucceedEventArgs(this));
            }
        }

        private void Order_DeclareSucceed(object sender, OrderDeclareSucceedEventArgs e)
        {
            if (e.Order.Admin != null)
            {
                //写入日志
                e.Order.Log(e.Order.Admin, "报关员[" + e.Order.Admin.RealName + "]完成订单报关，等待出库。");
                e.Order.Trace(e.Order.Admin, OrderTraceStep.Declaring, "您的订单报关完成");
            }
        }
    }

    /// <summary>
    /// 已报关，待出库的订单
    /// </summary>
    public class DeclaredOrder : Order
    {
        /// <summary>
        /// 是否已经通知出库(True: 订单型号已全部生成出库通知; False: 订单型号未全部生成出库通知)
        /// </summary>
        public bool HasNotified { get; set; }
        public string ExitNoticeId { get; set; }

        /// <summary>
        /// 当订单完成出库时发生
        /// </summary>
        public event OrderWarehouseExitedHanlder OrderWarehouseExited;
        public event OrderWarehouseExitedHanlder OrderWarehouseExiting;

        public DeclaredOrder()
        {
            this.OrderWarehouseExited += Order_WarehouseExited;
            this.OrderWarehouseExiting += Order_WarehouseExiting;
        }

        /// <summary>
        /// 订单出库
        /// </summary>
        public void WarehouseExit()
        {
            this.OnWarehouseExiting();
         
            //已出库数量
            var exitedQty = 0M;
            var lists = new Needs.Ccs.Services.Views.SubOrderExitQtyView(this.ID).ToList();
            if (lists.Count != 0)
            {
                exitedQty = lists.Sum(t => t.Quantity);
            }

            //全部出库完成
            if (this.Items.Sum(item => item.Quantity) == exitedQty)
            {
                //修改订单状态 --2019.12.10 LK 只要有一个出库通知，该订单就变为待收货状态，还是不等全都出库完了，才变为待出库
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = OrderStatus.WarehouseExited }, item => item.ID == this.ID);
                }
                this.OnWarehouseExited();
            }

        }

        virtual protected void OnWarehouseExited()
        {
            if (this != null && this.OrderWarehouseExited != null)
            {
                this.OrderWarehouseExited(this, new OrderWarehouseExitedEventArgs(this));
            }
        }

        private void Order_WarehouseExited(object sender, OrderWarehouseExitedEventArgs e)
        {
            if (e.Order.Admin != null)
            {
                //写入日志
                e.Order.Log(e.Order.Admin, "仓库人员[" + e.Order.Admin.RealName + "]完成订单出库，等待客户收货。");
            }
        }


        virtual protected void OnWarehouseExiting()
        {
            if (this != null && this.OrderWarehouseExiting != null)
            {
                this.OrderWarehouseExiting(this, new OrderWarehouseExitedEventArgs(this));
            }
        }

        private void Order_WarehouseExiting(object sender, OrderWarehouseExitedEventArgs e)
        {
            if (e.Order.Admin != null)
            {
                //写入日志
                //深圳库房出库 写入订单日志
                var ExiNotice = new Views.SZExitNoticeView().Where(item => item.ID == this.ExitNoticeId && item.ExitNoticeStatus == ExitNoticeStatus.Exited).FirstOrDefault();
                switch (ExiNotice.ExitType)
                {
                    case ExitType.PickUp:
                        e.Order.Log(e.Order.Admin, "深圳库房[" + e.Order.Admin.RealName + "]出库完成，提货人：" + ExiNotice.LadingBill.DeliveryName + "" + ExiNotice.LadingBill.DeliveryTel);
                        e.Order.Trace(e.Order.Admin, OrderTraceStep.PickUp, "出库完成，已提货（自提）提货人：" + ExiNotice.LadingBill.DeliveryName + "，" + ExiNotice.LadingBill.DeliveryTel);
                        break;
                    case ExitType.Delivery:
                        e.Order.Log(e.Order.Admin, "深圳库房[" + e.Order.Admin.RealName + "]出库完成，送货人：" + ExiNotice.DeliveryBill.DriverName + "" + ExiNotice.DeliveryBill.DriverTel + "" + ExiNotice.DeliveryBill.License);
                        e.Order.Trace(e.Order.Admin, OrderTraceStep.Delivering, "出库完成，您的订单正在派送中，送货人：" + ExiNotice.DeliveryBill.DriverName + "" + ExiNotice.DeliveryBill.DriverTel + "，" + ExiNotice.DeliveryBill.License);
                        break;
                    case ExitType.Express:
                        e.Order.Log(e.Order.Admin, "深圳库房[" + e.Order.Admin.RealName + "]待发货完成，快递单号：" + ExiNotice.ExpressBill.ExpressComp + "" + ExiNotice.ExpressBill.Code);
                        e.Order.Trace(e.Order.Admin, OrderTraceStep.Delivering, "出库完成，您的订单正在派送中，（" + ExiNotice.ExpressBill.ExpressComp + " 快递单号：<a target='_blank' href='https://www.kuaidi100.com/chaxun?com=" + ExiNotice.ExpressBill.QueryMark + "&nu=" + ExiNotice.ExpressBill.Code + "'>" + ExiNotice.ExpressBill.Code + "</a>）");
                        break;
                    default:
                        break;
                }

            }
        }
    }

    /// <summary>
    /// 已出库，待收货的订单
    /// </summary>
    public class WarehouseExitedOrder : Order
    {
        /// <summary>
        /// 当客户确认收货时发生
        /// </summary>
        public event OrderReceiveConfirmedHanlder ReceiveConfirmed;


        public WarehouseExitedOrder()
        {
            this.ReceiveConfirmed += Order_Received;
        }

        private void Order_Received(object sender, Hanlders.OrderReceiveConfirmedEventArgs e)
        {
            var order = (WarehouseExitedOrder)e.Order;
            //写入日志
            if (order.User != null)
            {
                order.Log(order.User, "用户[" + order.User.RealName + "]确认收货");
                order.Trace(order.User, OrderTraceStep.Completed, "您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作");
            }
        }
        void OnRecieveConfirmed()
        {
            if (this != null && this.ReceiveConfirmed != null)
            {
                this.ReceiveConfirmed(this, new OrderReceiveConfirmedEventArgs(this));
            }
        }

        /// <summary>
        /// 客户确认收货
        /// </summary>
        public void RecieveConfirm()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.Completed }, item => item.ID == this.ID);
            }
            this.OnRecieveConfirmed();
        }

    }

    /// <summary>
    /// 已完成订单
    /// </summary>
    public class CompletedOrder : Order
    {

    }

    /// <summary>
    /// 已退回的订单
    /// </summary>
    public class ReturnedOrder : Order
    {
        /// <summary>
        /// 订单取消原因
        /// </summary>
        public string CanceledSummary { get; set; }

        /// <summary>
        /// 当订单取消时发生
        /// </summary>
        public event OrderCanceledHanlder Canceled;

        public ReturnedOrder()
        {
            this.Canceled += Order_Canceled;
        }

        private void Order_Canceled(object sender, Hanlders.OrderCanceledEventArgs e)
        {
            var order = (ReturnedOrder)e.Order;
            if (order.Admin != null)
            {
                //写入日志
                e.Order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]取消了订单,订单取消原因：" + order.CanceledSummary);
                e.Order.Trace(Admin, OrderTraceStep.Anomaly, "您的订单已取消，取消原因：" + order.CanceledSummary);
            }
            if (order.User != null)
            {
                //写入日志
                e.Order.Log(order.User, "用户[" + order.User.RealName + "]取消了订单,订单取消原因：" + order.CanceledSummary);
                e.Order.Trace(Admin, OrderTraceStep.Anomaly, "您的订单已取消，取消原因：" + order.CanceledSummary);
            }

            //订单取消，删除该订单的收款数据
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //将订单货款、关税、增值税、代理费、商检费、杂费实收金额写回收款通知
                var receiveds = new Views.OrderReceivedsView().Where(item => item.OrderID == order.ID);
                var receiptNotices = receiveds.GroupBy(item => item.ReceiptNoticeID);
                receiptNotices.ToList().ForEach(item =>
                {
                    var clearAmount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>().First(rn => rn.ID == item.Key).ClearAmount;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new { ClearAmount = clearAmount - item.Sum(r => r.Amount) }, rn => rn.ID == item.Key);
                });

                //删除货款、关税、增值税、代理费、商检费、杂费收款记录
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderReceipts>(or => or.OrderID == order.ID);
            }
        }

        virtual protected void OnCanceled()
        {
            if (this != null && this.Canceled != null)
            {
                this.Canceled(this, new OrderCanceledEventArgs(this));
            }
        }

        /// <summary>
        /// 订单取消
        /// </summary>
        public void Cancel()
        {
            this.OrderStatus = Enums.OrderStatus.Canceled;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = Enums.OrderStatus.Canceled }, item => item.ID == this.ID);
            }

            this.OnCanceled();
        }
    }

    /// <summary>
    /// 已取消的订单
    /// </summary>
    public class CanceledOrder : Order
    {
        /// <summary>
        /// 订单取消原因
        /// </summary>
        public string CanceledSummary { get; set; }
    }

    /// <summary>
    /// 挂起的订单
    /// </summary>
    public class HangUpOrder : Order
    {
        /// <summary>
        /// 订单挂起原因
        /// </summary>
        public string HangUpReason
        {
            get
            {
                var reason = string.Empty;
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var controls = (from entity in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                                    join step in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>() on entity.ID equals step.OrderControlID
                                    where this.ID == entity.OrderID && step.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                                    select (Enums.OrderControlType)entity.ControlType).Distinct().ToList();
                    controls.ForEach(type =>
                    {
                        reason += type.GetDescription() + " ";
                    });
                }
                return reason;
            }
        }
    }

    /// <summary>
    /// 待付汇订单
    /// </summary>
    public class UnPayExchangeOrder : Order
    {
        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 本次付汇金额
        /// </summary>
        public decimal CurrentPaidAmount { get; set; }

        /// <summary>
        /// 汇率类型
        /// </summary>
        public int ExchangeRateType { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

    }

    /// <summary>
    /// 待开票订单
    /// </summary>
    public class UnInvoicedOrder : Order
    {

    }

    /// <summary>
    /// 销售合同订单
    /// </summary>
    public class SalesContractOrder : Order
    {

    }

    /// <summary>
    /// 待归类
    /// </summary>
    public class UnClassfiedOrder : Order
    {

    }
    /// <summary>
    /// 可维护费用的订单
    /// </summary>
    public class FeeMaintenanceOrder : Order
    {

    }

    public class UnCollectedOrder : Order
    {
        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }
       
    }
}
