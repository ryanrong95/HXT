using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Linq;
using Yahv.Services.Events;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.Services
{
    static public class Initializers
    {
        static object locker = new object();

        static object whsBoot;
        /// <summary>
        /// 代仓储订单
        /// </summary>
        static public void WhsBoot()
        {
            if (whsBoot == null)
            {
                lock (locker)
                {
                    if (whsBoot == null)
                    {
                        whsBoot = new object();
                        ReceivedBase.WhsConfirmed += WhsReceived_Confirmed;
                    }
                }
            }

        }

        private static object lsBoot;
        /// <summary>
        /// 租赁订单
        /// </summary>
        static public void LsBoot()
        {
            if (lsBoot == null)
            {
                lock (locker)
                {
                    if (lsBoot == null)
                    {
                        lsBoot = new object();
                        ReceivedBase.LsConfirmed += LsReceived_Confirmed;
                    }
                }
            }

        }

        private static object orderBoot;

        static public void OrderBoot()
        {
            if (orderBoot == null)
            {
                lock (locker)
                {
                    if (orderBoot == null)
                    {
                        orderBoot = new object();
                        ReceivedBase.Recording += OrderInfo;
                    }
                }
            }
        }

        /// <summary>
        /// 代仓储订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WhsReceived_Confirmed(object sender, ConfirmedEventArgs<WhsPayConfirmedEventArgs> e)
        {
            foreach (var s in e.Status)
            {
                //订单添加应收费用时，无需修改状态（除部分支付和已支付）
                if (s.Source == Enums.SourceType.Receivable)
                {
                    var currentPayStatus = new WsOrderStatusTopView<PvCenterReponsitory>()
                        .Where(item => item.MainID == s.OrderID && item.Type == OrderStatusType.PaymentStatus && item.IsCurrent == true).FirstOrDefault();
                    if (currentPayStatus == null || currentPayStatus.Status == (int)OrderPaymentStatus.Waiting ||
                        currentPayStatus.Status == (int)OrderPaymentStatus.Confirm || currentPayStatus.Status == (int)OrderPaymentStatus.ToBePaid)
                    {
                        continue;
                    }
                }
                //订单账单减免时：无需修改订单支付状态（除已支付）
                if (s.Source == Enums.SourceType.Reduction && s.Status != OrderPaymentStatus.Paid)
                {
                    continue;
                }
                //更新订单支付状态
                if (s.Status == OrderPaymentStatus.PartPaid || s.Status == OrderPaymentStatus.Paid)
                {
                    if (!string.IsNullOrWhiteSpace(s.OrderID))
                    {
                        Logs_PvWsOrder log = new Logs_PvWsOrder();
                        log.MainID = s.OrderID;
                        log.Type = OrderStatusType.PaymentStatus;
                        log.CreatorID = s.OperatorID;
                        log.Status = (int)s.Status;
                        log.Enter();
                    }
                }

                #region 收付款申请的收款确认
                if (s.Source == Enums.SourceType.Confirm && !string.IsNullOrEmpty(s.ApplicationID))
                {
                    using (var reponsitory = new PvWsOrderReponsitory())
                    {
                        var application = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationsTopView>()
                            .Where(item => item.ID == s.ApplicationID).FirstOrDefault();
                        if (s.Status == OrderPaymentStatus.Paid)
                        {
                            //if (application.Type == (int)ApplicationType.Receival)
                            //{
                            //    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                            //    {
                            //        ExcuteStatus = ApplicationStatus.Completed
                            //    }, item => item.ID == s.ApplicationID);
                            //    //TODO:账户转入客户余额，财务直接在确认收款里边做了,这边不再做
                            //}
                        }

                    }
                }
                #endregion

                #region 付款申请的付款确认
                if (s.Source == Enums.SourceType.ConfirmPayment && !string.IsNullOrEmpty(s.ApplicationID))
                {
                    using (var reponsitory = new PvWsOrderReponsitory())
                    {
                        var application = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationsTopView>()
                            .Where(item => item.ID == s.ApplicationID).FirstOrDefault();
                        if (s.Status == OrderPaymentStatus.Paid)
                        {
                            //if (application != null && application.Type == (int)ApplicationType.Payment)
                            //{
                            //    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Applications>(new
                            //    {
                            //        ExcuteStatus = ApplicationStatus.Completed
                            //    }, item => item.ID == s.ApplicationID);
                            //}
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 租赁订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void LsReceived_Confirmed(object sender, ConfirmedEventArgs<LsPayConfirmedEventArgs> e)
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                foreach (var s in e.Status)
                {
                    //更新订单状态为待分配
                    Logs_PvLsOrder log = new Logs_PvLsOrder();
                    log.MainID = s.LsOrderID;
                    log.Type = LsOrderStatusType.MainStatus;
                    log.CreatorID = s.OperatorID;
                    log.Status = (int)LsOrderStatus.UnAllocate;
                    log.Enter();
                    Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                    {
                        Status = (int)LsOrderStatus.UnAllocate,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == s.LsOrderID);

                    //生成库房的租赁通知
                    var lsOrder = new LsOrderTopView<PvLsOrderReponsitory>().Where(item => item.ID == s.LsOrderID).FirstOrDefault();
                    var lsOrderItems = new LsOrderItemTopView<PvLsOrderReponsitory>().Where(item => item.OrderID == s.LsOrderID)
                        .Where(item => item.Status != GeneralStatus.Closed || item.Status != GeneralStatus.Deleted);
                    var notices = lsOrderItems.Select(item => new LsNotice
                    {
                        SpecID = item.Product.SpecID,
                        Quantity = item.Quantity,
                        StartDate = item.Lease.StartDate,
                        EndDate = item.Lease.EndDate,
                        OrderID = lsOrder.ID,
                        ClientID = lsOrder.ClientID,
                        PayeeID = lsOrder.PayeeID,
                    }).ToArray();
                    var submitData = new LsNoticeSubmit { List = notices };//通知对象
                    //调用库房接口
                    var apiurl = ConfigurationManager.AppSettings["ApiWmsUrl"] + "lsnotice/submit";
                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, submitData);
                }
            }
        }

        /// <summary>
        /// 订单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OrderInfo(object sender, OrderEventArgs e)
        {
            try
            {
                if (sender != null && !string.IsNullOrWhiteSpace(e.OrderID))
                {
                    using (var orderView = new WsOrdersTopView<PvbCrmReponsitory>())
                    using (var lsOrderView = new LsOrderTopView<PvbCrmReponsitory>())
                    {
                        if (!e.OrderID.StartsWith("LsOrder"))
                        {
                            var order = orderView[e.OrderID];

                            if (order == null)
                            {
                                throw new Exception("未找到该订单信息!");
                            }

                            e.Currency = (Currency)order.SettlementCurrency;
                            e.OrderCreateDate = order.CreateDate;
                            e.ClientID = order.ClientID;
                        }
                        else
                        {
                            var order = lsOrderView[e.OrderID];

                            if (order == null)
                            {
                                throw new Exception("未找到该订单信息!");
                            }

                            e.Currency = order.Currency;
                            e.OrderCreateDate = order.CreateDate;
                            e.ClientID = order.ClientID;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //e.Currency = Currency.CNY;      //导入默认
            }
        }
    }
}
