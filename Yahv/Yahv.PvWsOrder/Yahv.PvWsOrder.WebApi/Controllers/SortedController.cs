using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class SortedController : ClientController
    {
        /// <summary>
        /// 库房分拣结果自动化更新
        /// </summary>
        /// <param name="OrderId">多个订单号orderid</param>
        /// <returns></returns>
        [HttpPost]
        [HttpPayload]
        public ActionResult SubmitSorted(JPost obj)
        {
            try
            {
                foreach (var item in obj.ToObject<StoreChange>().List)
                {
                    var order = new PvWsOrder.Services.Views.OrderAlls()[item.orderid];
                    if (order == null)
                    {
                        throw new Exception("订单" + item.orderid + "不存在");
                    }
                    if (order.Type == OrderType.Declare)
                    {
                        //报关到货由芯达通通知接口：SubmitChanged
                        var message = new JMessage() { code = 200, success = true, data = "提交成功" };
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                    //TODO：待测试 日志跟踪库房到货调用情况  
                    using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
                    {
                        reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Operating()
                        {
                            ID = Guid.NewGuid().ToString(),
                            Type = (int)Yahv.Services.Enums.LogType.WsOrder,
                            MainID = order.ID,
                            Operation = "库房调用SubmitSorted接口，更新分拣到货项至订单项",
                            Creator = "NPC",
                            CreateDate = DateTime.Now,
                            Summary = item.Json(),
                        });
                        reponsitory.Submit();
                    }
                    //保存订单的到货信息
                    order.SaveOrderDeliveries(item.orderid);
                }
                var json = new JMessage() { code = 200, success = true, data = "提交成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "提交失败" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 芯达通系统将变更后的报关订单数据通给代仓储
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitChanged(JPost obj)
        {
            try
            {
                var result = obj.ToObject<OrderChanges>();
                //日志跟踪库房到货调用情况
                Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = result.OrderID,
                    Operation = "芯达通调用SubmitChanged接口，同步到货信息。",
                    Summary = result.Json(),
                });

                using (var reponsitory = new PvWsOrderReponsitory())
                {
                    //查询订单项数据
                    var orderitemAlls = new Services.Views.OrderItemsRoll(result.OrderID, reponsitory);
                    var changes = result.items.Select(en => en.OrderItemID);
                    //是否存在退回之前备份数据
                    foreach (var itemid in changes)
                    {
                        if (reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().Any(item => item.ID == "OOT" + itemid) &&
                            reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().Any(item => item.ID == itemid && item.Type == (int)Services.Enums.OrderItemType.Normal))
                        {
                            reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(en => en.ID == "OOT" + itemid);
                            reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(en => en.ID == "OOT" + itemid);
                            reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItems>(en => en.ID == "OOT" + itemid);
                        }
                    }

                    //修改下单时订单项ID的前缀为OOT
                    var normalUpdates = orderitemAlls.Where(item => changes.Contains(item.ID) && item.Type == Services.Enums.OrderItemType.Normal).ToArray();
                    foreach (var item in normalUpdates)
                    {
                        var id = item.ID;
                        item.ID = "OOT" + id;
                        reponsitory.Insert(item.ToLinq());
                        if (item.OrderItemsChcd != null)
                        {
                            item.OrderItemsChcd.ID = item.ID;
                            reponsitory.Insert(item.OrderItemsChcd.ToLinq());
                        }
                        if (item.OrderItemsTerm != null)
                        {
                            item.OrderItemsTerm.ID = item.ID;
                            reponsitory.Insert(item.OrderItemsTerm.ToLinq());
                        }
                        //更新到货
                        reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            ModifyDate = DateTime.Now,
                            Type = (int)Services.Enums.OrderItemType.Modified,
                        }, en => en.ID == id);
                    }

                    #region 更新到货信息
                    var modifiedIds = orderitemAlls.Where(item => item.Type == Services.Enums.OrderItemType.Modified).Select(en => en.ID).ToArray();
                    var modifiedInsertIds = changes.Where(item => !modifiedIds.Contains(item)).ToArray();
                    var modifiedUpdateIds = modifiedIds.Where(item => changes.Contains(item)).ToArray();
                    var modifiedDeleteIds = modifiedIds.Where(item => !changes.Contains(item)).ToArray();

                    //新增到货信息
                    List<Layers.Data.Sqls.PvWsOrder.OrderItems> orderItems = new List<Layers.Data.Sqls.PvWsOrder.OrderItems>();
                    foreach (var id in modifiedInsertIds)
                    {
                        var rel = result.items.Single(t => t.OrderItemID == id);
                        orderItems.Add(new Layers.Data.Sqls.PvWsOrder.OrderItems
                        {
                            ID = rel.OrderItemID,
                            OrderID = result.OrderID,
                            TinyOrderID = rel.TinyOrderID,
                            InputID = rel.InputID,
                            CustomName = rel.CustomName,
                            ProductID = rel.ProductID,
                            Origin = rel.OriginEx.GetOrigin().Code,
                            DateCode = rel.DateCode,
                            Quantity = rel.Quantity,
                            UnitPrice = rel.UnitPrice,
                            Currency = (int)result.CurrencyEx,
                            GrossWeight = rel.GrossWeight,
                            TotalPrice = rel.TotalPrice,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            Unit = (int)LegalUnit.个,
                            Volume = 0.01m,
                            Conditions = new Services.Models.OrderItemCondition().Json(),
                            Status = (int)GeneralStatus.Normal,
                            IsAuto = false,
                            Type = (int)Services.Enums.OrderItemType.Modified,
                        });
                    }
                    reponsitory.Insert(orderItems.ToArray());
                    //更新到货信息
                    foreach (var id in modifiedUpdateIds)
                    {
                        var rel = result.items.Where(en => en.OrderItemID == id).FirstOrDefault();
                        if (string.IsNullOrEmpty(rel.InputID))
                        {
                            //如果InputID为空（即库房未到货）则不更新InputID；原因芯达通取不到
                            reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                            {
                                TinyOrderID = rel.TinyOrderID,
                                ProductID = rel.ProductID,
                                Origin = rel.OriginEx.GetOrigin().Code,
                                DateCode = rel.DateCode,
                                Quantity = rel.Quantity,
                                UnitPrice = rel.UnitPrice,
                                TotalPrice = rel.TotalPrice,
                            }, en => en.ID == id);
                        }
                        else
                        {
                            //开始更新
                            reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                            {
                                TinyOrderID = rel.TinyOrderID,
                                InputID = rel.InputID,
                                ProductID = rel.ProductID,
                                Origin = rel.OriginEx.GetOrigin().Code,
                                DateCode = rel.DateCode,
                                Quantity = rel.Quantity,
                                UnitPrice = rel.UnitPrice,
                                TotalPrice = rel.TotalPrice,
                            }, en => en.ID == id);
                        }
                    }

                    foreach(var item in result.OriginOrderItemIDs)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            InputID = Layers.Data.PKeySigner.Pick(PKeyType.Input),                           
                        }, en => en.ID == item);
                    }

                    //物理删除到货信息
                    reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(en => modifiedDeleteIds.Contains(en.ID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(en => modifiedDeleteIds.Contains(en.ID));
                    reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.OrderItems>(en => modifiedDeleteIds.Contains(en.ID));

                    var totalprice = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().Where(item => item.OrderID == result.OrderID && item.Type == 2).Sum(item => item.TotalPrice);

                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                    {
                        TotalPrice = totalprice,
                    }, item => item.ID == result.OrderID);
                    #endregion
                }
                //返回结果
                var json = new JMessage() { code = 200, success = true, data = "提交成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "提交失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
