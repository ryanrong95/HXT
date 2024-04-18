using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;


namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SZWarehouseController : MyApiController
    {

        /// <summary>
        /// 深圳出库完成操作
        /// </summary>
        /// <param name="AdminID">操作人 </param>
        /// <param name="WaybillID">运单ID</param>
        /// <returns></returns>
        public ActionResult OutStock(string AdminID, string WaybillID)
        {
            try
            {
                var json = new JMessage();
                if (string.IsNullOrEmpty(WaybillID))
                {
                    json.code = 100;
                    json.success = false;
                    json.data = "参数WaybillID为空!";
                }
                if (string.IsNullOrEmpty(AdminID))
                {
                    json.code = 100;
                    json.success = false;
                    json.data = "参数AdminID为空!";
                }

                //记录日志
                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    RequestContent = "AdminID:" + AdminID + ",WaybillID:" + WaybillID,
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "深圳库房调用出库完成"
                };
                apiLog.Enter();

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var originAmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.ErmAdminID == AdminID);
                    var admin = new Admin
                    {
                        ID = originAmin.OriginID,
                        UserName = originAmin.UserName,
                        RealName = originAmin.RealName,

                    };

                    //
                    var exitwaybill = reponsitory.ReadTable<CgSzOutputWaybillsTopView>().FirstOrDefault(t => t.WaybillID == WaybillID);
                    if (exitwaybill != null)
                    {
                        var StockInfo = (from c in reponsitory.ReadTable<CgSZOutputDetail>()
                                         where c.WaybillID == WaybillID
                                         select new
                                         {
                                             OrderID = c.TinyOrderID,
                                             OrderItemID = c.ItemID,
                                         }).ToList();

                        var allbill = from billitem in reponsitory.ReadTable<CgSZOutputDetail>()
                                      join bill in reponsitory.ReadTable<CgSzOutputWaybillsTopView>() on billitem.WaybillID equals bill.WaybillID
                                      where bill.IsModify == (int)CgPickingExcuteStatus.Completed
                                      select new
                                      {
                                          bill.WaybillID,
                                          billitem.Quantity,
                                          billitem.OrderID
                                      };

                        var exitbill = allbill.Where(t => t.WaybillID == WaybillID).ToList();
                        //主订单号
                        var mainorderids = exitbill.Select(t => t.OrderID).Distinct().ToArray();


                        var allexit = 0M;
                        var orderqty = 0M;

                        foreach (var orderid in mainorderids)
                        {
                            allexit += allbill.Where(t => t.OrderID == orderid).Sum(t => t.Quantity);

                            //查询主订单的所有型号的数量
                            orderqty += (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                         join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                         on c.ID equals d.OrderID
                                         where c.MainOrderId == orderid && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                                         select d.Quantity).Sum();

                            if (allexit == orderqty)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = OrderStatus.WarehouseExited }, item => item.MainOrderId == orderid);

                                //写日志
                                var OrderIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                                where c.MainOrderId == orderid && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                                                select c.ID).ToList();

                                foreach (var oid in OrderIDs)
                                {
                                    //记录日志
                                    OrderLog log = new OrderLog();
                                    log.OrderID = oid;
                                    log.Admin = admin;
                                    log.OrderStatus = OrderStatus.WarehouseExited;
                                    log.Summary = "仓库人员[" + admin.RealName + "]完成订单出库，等待客户收货。";
                                    log.Enter();
                                }
                            }
                        }


                        #region 插入发出商品
                        Task.Run(() =>
                        {
                            IOutGoodsAdd outGoodsAdd = new StockOut();
                            outGoodsAdd.OrderID = StockInfo.FirstOrDefault().OrderID;
                            outGoodsAdd.OrderItems = new List<string>();
                            foreach (var item in StockInfo)
                            {
                                outGoodsAdd.OrderItems.Add(item.OrderItemID);
                            }
                            OutGoodsContext outGoodsContext = new OutGoodsContext(outGoodsAdd);
                            outGoodsContext.context();
                        });
                        #endregion

                        //var exitedQty = allbill.Where(t => t.OrderID == mainorderid).Sum(t => t.Quantity);
                        ////var exitedQty = allbill.Sum(t => t.Quantity);

                        ////查询主订单的所有型号的数量
                        //var orderQty = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        //                join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                        //                on c.ID equals d.OrderID
                        //                where c.MainOrderId == mainorderid && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                        //                select d.Quantity).Sum();

                        //if (exitedQty == orderQty)
                        //{
                        //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = OrderStatus.WarehouseExited }, item => item.MainOrderId == mainorderid);

                        //    //写日志
                        //    var OrderIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        //                    where c.MainOrderId == mainorderid && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                        //                    select c.ID).ToList();

                        //    foreach (var orderid in OrderIDs)
                        //    {
                        //        //记录日志
                        //        OrderLog log = new OrderLog();
                        //        log.OrderID = orderid;
                        //        log.Admin = admin;
                        //        log.OrderStatus = OrderStatus.WarehouseExited;
                        //        log.Summary = "仓库人员[" + admin.RealName + "]完成订单出库，等待客户收货。";
                        //        log.Enter();
                        //    }
                        //}

                        json = new JMessage()
                        {
                            code = 200,
                            success = true,
                            data = "调用成功"
                        };

                        apiLog.OrderID = string.Join(",", mainorderids);
                    }
                    else
                    {
                        json.code = 100;
                        json.success = false;
                        json.data = "查询不到该运单!";
                    }

                }

                apiLog.ResponseContent = json.data;
                apiLog.Enter();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("深圳库房出库调用完成出库接口");
                var json = new JMessage() { code = 300, success = false, data = "出库失败，" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 确认收货操作
        /// </summary>
        /// <param name="AdminID">操作人</param>
        /// <param name="WaybillID">运单ID</param>
        /// <returns></returns>
        public ActionResult ReceiptConfirmed(string AdminID, string WaybillID)
        {
            try
            {
                var json = new JMessage();
                if (string.IsNullOrEmpty(WaybillID))
                {
                    json.code = 100;
                    json.success = false;
                    json.data = "参数WaybillID为空!";
                }
                if (string.IsNullOrEmpty(AdminID))
                {
                    json.code = 100;
                    json.success = false;
                    json.data = "参数AdminID为空!";
                }

                //记录日志
                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    RequestContent = "AdminID:" + AdminID + ",WaybillID:" + WaybillID,
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "调用客户确认收货"
                };
                apiLog.Enter();

                //开始将出库通知从“已出库”变为“已完成”，并且看订单状态是否可以变为“已完成”和是否需要插入订单日志和订单轨迹
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var originAmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.ErmAdminID == AdminID);
                    var admin = new Admin
                    {
                        ID = originAmin.OriginID,
                        UserName = originAmin.UserName,
                        RealName = originAmin.RealName,

                    };

                    //
                    var exitwaybill = reponsitory.ReadTable<CgSzOutputWaybillsTopView>().FirstOrDefault(t => t.WaybillID == WaybillID);
                    if (exitwaybill == null)
                    {
                        json.code = 100;
                        json.success = false;
                        json.data = "查询不到该运单!";
                    }
                    else
                    {
                        var allbill = from billitem in reponsitory.ReadTable<CgSZOutputDetail>()
                                      join bill in reponsitory.ReadTable<CgSzOutputWaybillsTopView>() on billitem.WaybillID equals bill.WaybillID
                                      where bill.ConfirmReceiptStatus == (int)CgConfirmReceiptStatus.Comfirmed
                                      select new
                                      {
                                          bill.WaybillID,
                                          billitem.Quantity,
                                          billitem.OrderID
                                      };

                        var exitbill = allbill.Where(t => t.WaybillID == WaybillID).ToList();
                        //主订单号
                        var mainorderids = exitbill.Select(t => t.OrderID).Distinct().ToArray();

                        var allexit = 0M;
                        var orderqty = 0M;

                        foreach (var orderid in mainorderids)
                        {
                            allexit += allbill.Where(t => t.OrderID == orderid).Sum(t => t.Quantity);

                            //查询主订单的所有型号的数量
                            orderqty += (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                         join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                         on c.ID equals d.OrderID
                                         where c.MainOrderId == orderid && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                                         select d.Quantity).Sum();

                            if (allexit == orderqty)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                                new { OrderStatus = (int)Needs.Ccs.Services.Enums.OrderStatus.Completed },
                                item => item.MainOrderId == orderid
                                        && item.OrderStatus != (int)OrderStatus.Canceled
                                        && item.OrderStatus != (int)OrderStatus.Returned);

                                //写日志
                                var OrderIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                                where c.MainOrderId == orderid && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                                                select c.ID).ToList();

                                foreach (var oid in OrderIDs)
                                {
                                    string thisOrderID = oid;

                                    // OrderLogs 表插入日志
                                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderLogs>(new Layer.Data.Sqls.ScCustoms.OrderLogs()
                                    {
                                        ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderLog),
                                        OrderID = thisOrderID,
                                        AdminID = AdminID,
                                        OrderStatus = (int)OrderStatus.Completed,
                                        CreateDate = DateTime.Now,
                                        Summary = "该订单已出库完成，已将订单状态置为【完成】",
                                    });

                                    int targetCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                                        .Where(t => t.OrderID == thisOrderID
                                                 && t.Step == (int)OrderTraceStep.Completed).Count();
                                    if (targetCount <= 0)
                                    {
                                        // OrderTraces 表中插入订单轨迹
                                        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderTraces>(new Layer.Data.Sqls.ScCustoms.OrderTraces()
                                        {
                                            ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderTrace),
                                            OrderID = thisOrderID,
                                            AdminID = AdminID,
                                            Step = (int)OrderTraceStep.Completed,
                                            CreateDate = DateTime.Now,
                                            Summary = "您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作",
                                        });
                                    }
                                }
                            }
                        }

                        //var confirmedQty = allbill.Sum(t => t.Quantity);

                        ////查询主订单的所有型号的数量
                        //var orderQty = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        //                join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                        //                on c.ID equals d.OrderID
                        //                where c.MainOrderId == exitwaybill.OrderID && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                        //                select d.Quantity).Sum();



                        //判断两个数量是否相等
                        //if (orderQty == confirmedQty)
                        //{

                        //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                        //        new { OrderStatus = (int)Needs.Ccs.Services.Enums.OrderStatus.Completed },
                        //        item => item.MainOrderId == exitwaybill.OrderID
                        //                && item.OrderStatus != (int)OrderStatus.Canceled
                        //                && item.OrderStatus != (int)OrderStatus.Returned);

                        //    //写日志
                        //    var OrderIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        //                    where c.MainOrderId == exitwaybill.OrderID && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                        //                    select c.ID).ToList();

                        //    foreach (var orderid in OrderIDs)
                        //    {
                        //        string thisOrderID = orderid;

                        //        // OrderLogs 表插入日志
                        //        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderLogs>(new Layer.Data.Sqls.ScCustoms.OrderLogs()
                        //        {
                        //            ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderLog),
                        //            OrderID = thisOrderID,
                        //            AdminID = AdminID,
                        //            OrderStatus = (int)OrderStatus.Completed,
                        //            CreateDate = DateTime.Now,
                        //            Summary = "该订单已出库完成，已将订单状态置为【完成】",
                        //        });

                        //        int targetCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                        //            .Where(t => t.OrderID == thisOrderID
                        //                     && t.Step == (int)OrderTraceStep.Completed).Count();
                        //        if (targetCount <= 0)
                        //        {
                        //            // OrderTraces 表中插入订单轨迹
                        //            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderTraces>(new Layer.Data.Sqls.ScCustoms.OrderTraces()
                        //            {
                        //                ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderTrace),
                        //                OrderID = thisOrderID,
                        //                AdminID = AdminID,
                        //                Step = (int)OrderTraceStep.Completed,
                        //                CreateDate = DateTime.Now,
                        //                Summary = "您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作",
                        //            });
                        //        }
                        //    }
                        //}

                        json.code = 200;
                        json.success = true;
                        json.data = "调用成功!";

                        apiLog.OrderID = string.Join(",", mainorderids);
                    }

                    apiLog.ResponseContent = json.ToString();
                    apiLog.Enter();

                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "操作失败:" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
