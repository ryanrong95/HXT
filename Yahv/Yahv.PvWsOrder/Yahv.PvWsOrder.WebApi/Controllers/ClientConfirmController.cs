using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsOrder.WebApi.Models;
using Yahv.Underly;
using Layers.Data.Sqls;
using Yahv.Utils.Serializers;
using Newtonsoft.Json.Linq;
using Yahv.Web.Mvc;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class ClientConfirmController : Controller
    {
        /// <summary>
        /// 测试用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult abc()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Orders()
                {
                    ID = "abc",
                    Type = 1,
                    ClientID = "",
                    InvoiceID = "",
                    PayeeID = "",
                    BeneficiaryID = "",
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Summary = "",
                    CreatorID = "",
                    SupplierID = "",
                    SortingInfo = "",
                    SettlementCurrency = 2,
                    TotalPrice = 111,
                });

                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderItems()
                {
                    ID = "999",
                    OrderID = "abc",
                    TinyOrderID = "",
                    InputID = "",
                    OutputID = "",
                    ProductID = "",
                    CustomName = "",
                    Origin = "",
                    DateCode = "",
                    Quantity = 123,
                    Currency = 1,
                    UnitPrice = 456,
                    Unit = 1,
                    TotalPrice = 23432,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    GrossWeight = 1241,
                    Volume = 432,
                    Conditions = "",
                    Status = 400,
                    IsAuto = true,
                    WayBillID = "",
                    Type = 1,
                    StorageID = "",
                });

                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                {
                    Summary = "1233434",
                }, item => item.ID == "abc");

                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                {
                    Status=400,
                    Origin="dsfoASDf234",
                }, item => item.ID == "999");

                var json = new JMessage() { code = 200, success = true, data = "提交成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 客户确认芯达通对接接口
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClientConfirm(JPost obj)
        {
            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            using (PvWsOrderReponsitory orderReponsitory = new PvWsOrderReponsitory())
            {
                try
                {
                    var confirm = obj.ToObject<Models.ClientConfirm>();
                    //传输日志记录
                    Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                    {
                        MainID = confirm.OrderID,
                        Operation = "客户确认芯达通对接",
                        Summary = confirm.Json(),
                    });
                    var ConfirmStatus = CgOrderStatus.待确认;

                    #region 报关标志处理
                    //报关标注不存在，不处理
                    if (confirm.DeclareFlags != null)
                    {
                        //批量处理
                        foreach (var flag in confirm.DeclareFlags)
                        {
                            var condition = orderReponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().FirstOrDefault(item => item.TinyOrderID == flag.TinyOrderID)?.Conditions;
                            if (string.IsNullOrEmpty(condition))
                            {
                                continue;
                            }
                            //反序列化,给报关标志赋值
                            var itemcondition = condition.JsonTo<OrderItemCondition>();
                            itemcondition.IsDeclare = flag.IsDeclare;
                            orderReponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                            {
                                Conditions = itemcondition.Json(),
                            }, item => item.TinyOrderID == flag.TinyOrderID);
                        }
                    }
                    #endregion


                    #region 数据修改对接操作
                    if (confirm.Type == ConfirmType.UpdateQuantity)
                    {
                        //查询当前订单状态
                        var OrderStatus = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>().FirstOrDefault(item => item.MainID == confirm.OrderID && item.Type == (int)OrderStatusType.MainStatus && item.IsCurrent == true).Status;
                        ConfirmStatus = OrderStatus > (int)CgOrderStatus.待确认 ? ConfirmStatus : (CgOrderStatus)OrderStatus;
                        var orderitem = orderReponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(item => item.ID == confirm.OrderItemID);
                        orderReponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            Quantity = confirm.Quantity,
                            ModifyDate = DateTime.Now,
                            TotalPrice = orderitem.UnitPrice * confirm.Quantity,
                        }, item => item.ID == confirm.OrderItemID);
                        //更新订单的总金额
                        if(orderReponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().
                            Any(item => item.OrderID == confirm.OrderID && item.Type == 2))
                        {
                            var totalprice = orderReponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().Where(item => item.OrderID == confirm.OrderID && item.Type == 2).Sum(item => item.TotalPrice);
                            orderReponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                            {
                                TotalPrice = totalprice,
                            }, item => item.ID == confirm.OrderID);
                        }
                        else
                        {
                            var totalprice = orderReponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().
                                Where(item => item.OrderID == confirm.OrderID).Sum(item => item.TotalPrice);
                            orderReponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                            {
                                TotalPrice = totalprice,
                            }, item => item.ID == confirm.OrderID);
                        }
                    }
                    if (confirm.Type == ConfirmType.DeleteItem)
                    {
                        //查询当前订单状态
                        var OrderStatus = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>().FirstOrDefault(item => item.MainID == confirm.OrderID && item.Type == (int)OrderStatusType.MainStatus && item.IsCurrent == true).Status;
                        ConfirmStatus = OrderStatus > (int)CgOrderStatus.待确认 ? ConfirmStatus : (CgOrderStatus)OrderStatus;
                        orderReponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            ModifyDate = DateTime.Now,
                            Status = (int)OrderItemStatus.Deleted,
                        }, item => item.ID == confirm.OrderItemID);
                    }
                    #endregion

                    //等待客户确认
                    if (confirm.Type != ConfirmType.DoNothing && ConfirmStatus == CgOrderStatus.待确认)
                    {
                        //当前确认状态置为失效
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, item => item.MainID == confirm.OrderID && item.Type == (int)OrderStatusType.MainStatus && item.IsCurrent == true);
                        reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = confirm.OrderID,
                            Type = (int)OrderStatusType.MainStatus,
                            Status = (int)ConfirmStatus,
                            CreateDate = DateTime.Now,
                            CreatorID = confirm.AdminID,
                            IsCurrent = true,
                        });
                        //当前确认状态置为失效
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false,
                        }, item => item.MainID == confirm.OrderID && item.Type == (int)OrderStatusType.PaymentStatus && item.IsCurrent == true);
                        reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = confirm.OrderID,
                            Type = (int)OrderStatusType.PaymentStatus,
                            Status = (int)OrderPaymentStatus.Confirm,
                            CreateDate = DateTime.Now,
                            CreatorID = confirm.AdminID,
                            IsCurrent = true,
                        });
                    }

                    //直接确认
                    if (confirm.Type == ConfirmType.DoNothing)
                    {
                        //直接发送入库通知
                        var orderExtends = new Yahv.PvWsOrder.Services.ClientViews.OrderAlls(orderReponsitory).GetDecNoticeDataByOrderID(confirm.OrderID);
                        if (orderExtends.Type == OrderType.Declare)
                        {
                            var message = orderExtends.CgDecEntryNotice();

                            Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                            {
                                MainID = confirm.OrderID,
                                Operation = "报关业务自动发入库通知",
                                Summary = message,
                            });
                        }
                        //else
                        //{
                        //    var message = orderExtends.CgDecBoxingNotice();
                        //    Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                        //    {
                        //        MainID = confirm.OrderID,
                        //        Operation = "报关业务自动发装箱通知",
                        //        Summary = message,
                        //    });
                        //}
                    }

                    //返回归类信息
                    var json = new JMessage() { code = 200, success = true, data = "提交成功" };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    //错误日志记录
                    Yahv.PvWsOrder.Services.Logger.Error("NPC", ex.Message);
                    var json = new JMessage() { code = 300, success = false, data = ex.Message };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
