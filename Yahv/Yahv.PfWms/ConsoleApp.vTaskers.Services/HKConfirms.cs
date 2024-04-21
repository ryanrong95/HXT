//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Yahv.Services;
//using Yahv.Services.Enums;
//using Yahv.Underly;

//namespace ConsoleApp.vTaskers.Services
//{
//    /// <summary>
//    /// 深圳出库:快递的7天后自动改为已确认收货。
//    /// </summary>
//    /// <remarks>
//    /// 模拟一个单例模式，完成定期执行
//    /// 运行逻辑是把深圳快递运单7天后还没有确认的订单确认信息生成
//    /// 1)	香港代发货业务，拣货出库后，直接发货给客户，所以客户确认收货后，直接更新订单的物流状态和运单的物流状态
//    /// 2)	深圳报关业务，跟单会将一个订单拆开成几单去发货给客户，因此会产生多个发货运单，客户需要根据实际到货的运单确认收货，从而更新运单的收货状态，等所有货物全都到货了，才能更新订单的状态
//    /// </remarks>
//    public class HKConfirms
//    {
//        public HKConfirms() { }

//        /// <summary>
//        /// 确认天数
//        /// </summary>
//        int confirmDaysCount = 7;

//        /// <summary>
//        /// 调用任务
//        /// </summary>
//        /// <param name="waybillID"></param>
//        internal void Task()
//        {
//            //快递的7天后自动改为已确认收货。

//            //深圳出库的情况:深圳报关业务，跟单会将一个订单拆开成几单去发货给客户，因此会产生多个发货运单，
//            //客户需要根据实际到货的运单确认收货，从而更新运单的收货状态，等所有货物全都到货了，才能更新订单的状态

//            using (var pvwms = new PvWmsRepository())
//            using (var pvcenter = new PvCenterReponsitory())
//            using (var pvwsorder = new PvWsOrderReponsitory())
//            {

//                //逻辑流程
//                #region 逻辑流程
//                /*
//                首先看库房的waybill是否有快递的，快递类型包涵：国际、本地
//                如果考虑香港与深圳分开处理是可以的，同时：我建议分开处理

//                地区自行判断
//                首先获取没有确认 ConfirmReceiptStatus   的正常出库waybill，前1000条，按照时间顺序排列

//                判断运单的最后修改时间ModifyDate，是否已经超过7天，没有超过的就什么都不做
//                超过7天需要完成收货确认  修改 waybill 的 ConfirmReceiptStatus

//                从waybill中获取input 与 output中的全部OrderID，并排重

//                如果完成以上更新
//                就需要重新读取订单的Item数据
//                如果都一致，就更新订单的状态为 ：CgOrderStatus.客户已收货 = 700, //客户确认收货
//                做状态累加
//                */
//                #endregion


//                //地区自行判断:获得深圳库房的waybill。首先获取没有确认 ConfirmReceiptStatus   的正常出库并且类型是 本地快递 的waybill，前1000条，按照时间顺序排列
//                var linq = from waybill in pvwms.ReadTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>()
//                           where waybill.WareHouseID.StartsWith(nameof(WhSettings.HK))//深圳库房
//                                && (waybill.ConfirmReceiptStatus == 100 || waybill.ConfirmReceiptStatus == null)//未确认
//                                && waybill.wbStatus == 200 //   正常出库 
//                                && waybill.wbExcuteStatus == (int)CgPickingExcuteStatus.Completed //完成装运
//                                && waybill.NoticeType == (int)CgNoticeType.Out //通知类型必须是出库订单
//                                && (waybill.wbType == (int)WaybillType.LocalExpress || waybill.wbType == (int)WaybillType.InternationalExpress)
//                                && (waybill.Source == (int)CgNoticeSource.AgentSend || waybill.Source == (int)CgNoticeSource.Transfer)
//                           orderby waybill.wbID descending
//                           select new
//                           {
//                               ID = waybill.wbID,
//                               OrderID = waybill.OrderID,
//                               CreateDate = waybill.wbCreateDate,
//                               ModifyDate = waybill.wbModifyDate,
//                               ConfirmReceiptStatus = waybill.ConfirmReceiptStatus,
//                               Status = waybill.wbStatus,
//                               Type = (WaybillType)waybill.wbType
//                           };


//                var waybills = linq.Take(1000).ToArray();

//                foreach (var waybill in waybills)
//                {
//                    var lastDate = waybill.ModifyDate ?? waybill.CreateDate;
//                    //快递七天后更新中心的Waybill信息
//                    if (lastDate.AddDays(confirmDaysCount) <= DateTime.Now)
//                    {
//                        pvcenter.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
//                        {
//                            ConfirmReceiptStatus = 200     //已确认收货
//                        }, item => item.ID == waybill.ID); //更新中心的waybill
//                    }
//                    else
//                    {
//                        continue;
//                    }

//                    //获取通知的数据
//                    var linq_notice = from notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//                                      join bill in waybills on notice.ID equals bill.ID
//                                      where bill.OrderID == waybill.OrderID
//                                      select new
//                                      {
//                                          notice.ID,
//                                          SendedQuantity = notice.Quantity, //通知的数量
//                                          notice.InputID, //通知的进项ID
//                                      };

//                    //分批出库产生新的出库通知但是对应的InputID是一样的
//                    var ienums_notice = (from item in linq_notice.ToArray()
//                                         group item by item.InputID into groups
//                                         orderby groups.Key ascending
//                                         select new
//                                         {
//                                             InputID = groups.Key,
//                                             NoticeTotal = groups.Sum(g => g.SendedQuantity)
//                                         }).ToArray();

//                    //获取订单数据
//                    var linq_orderItems = from item in pvwsorder.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
//                                          where item.OrderID == waybill.OrderID
//                                          select new
//                                          {
//                                              item.ID,
//                                              NoticeQuantity = item.Quantity,
//                                              item.InputID,
//                                              item.Type,
//                                          };
//                    var ienums_orderItem = (from item in linq_orderItems.ToArray()
//                                            where item.Type == 2
//                                            group item by item.InputID into groups
//                                            orderby groups.Key ascending
//                                            select new
//                                            {
//                                                InputID = groups.Key,
//                                                NoticeTotal = groups.Sum(g => g.NoticeQuantity)
//                                            }).ToArray();

//                    //发货的进项与订单的进项种类（多少个InputID） 一致
//                    if (ienums_notice.Length == ienums_orderItem.Length)
//                    {

//                        if (ienums_notice.Select((item, index) =>
//                        {
//                            return ienums_notice[index].NoticeTotal == ienums_orderItem[index].NoticeTotal;
//                        }).All(item => item))
//                        {
//                            pvcenter.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
//                            {
//                                IsCurrent = false
//                            }, item => waybill.OrderID == (item.MainID) && item.Type == (int)OrderStatusType.MainStatus);

//                            //订单日志修改状态为  客户已收货
//                            pvcenter.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
//                            {
//                                ID = Guid.NewGuid().ToString(),
//                                MainID = waybill.OrderID,
//                                Type = (int)OrderStatusType.MainStatus,
//                                Status = (int)CgOrderStatus.客户已收货,
//                                CreateDate = DateTime.Now,
//                                CreatorID = Npc.Robot.Obtain(),//系统自动
//                                IsCurrent = true
//                            });
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
