using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HKWarehouseController : MyApiController
    {
        /// <summary>
        /// 到货通知
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryNotice(JPost apiRequest)
        {
            var applies = apiRequest.ToObject<DeliveryNoticeRequest>();
            DeliveryNoticeReponse apiResponse = new DeliveryNoticeReponse();
            string batchID = Guid.NewGuid().ToString("N");
            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = batchID,
                OrderID = applies.VastOrderID,
                RequestContent = apiRequest.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = "库房推送到货信息"
            };

            //var deliveryNoticeApiHandler = new Needs.Ccs.Services.Models.DeliveryNoticeApiHandler(apiRequest.VastOrderID, batchID);
            //if (deliveryNoticeApiHandler.IsReturnOK)
            //{
            //    apiLog.ResponseContent = apiResponse.Json();
            //    apiLog.Enter();
            //    return Json(apiResponse, JsonRequestBehavior.AllowGet);
            //}

            //if (!deliveryNoticeApiHandler.IsOK)
            //{
            //    apiResponse.Success = false;
            //    apiResponse.Msg = deliveryNoticeApiHandler.Msg;
            //    apiLog.ResponseContent = apiResponse.Json();
            //    apiLog.Enter();
            //    return Json(apiResponse, JsonRequestBehavior.AllowGet);
            //}

            //deliveryNoticeApiHandler.GenerateOrderItemChangeAndSplit();

            //if (!deliveryNoticeApiHandler.IsOK)
            //{
            //    apiResponse.Success = false;
            //    apiResponse.Msg = deliveryNoticeApiHandler.Msg;
            //    apiLog.ResponseContent = apiResponse.Json();
            //    apiLog.Enter();
            //    return Json(apiResponse, JsonRequestBehavior.AllowGet);
            //}

            //UpdateSortingItemID(batchID, deliveryNoticeApiHandler.NewOrderItemPairs.ToArray());


            //发送当前订单信息
            //SendCurrentOrderInfo(batchID,
            //                     apiRequest.VastOrderID,
            //                     deliveryNoticeApiHandler.OldOrderItemIDs, deliveryNoticeApiHandler.NewOrderItemPairs.ToArray(),
            //                     deliveryNoticeApiHandler.OrderItemInView返回代仓储, deliveryNoticeApiHandler.OrderItemInLocal返回代仓储);

            SendCurrentOrderInfoNew(batchID, applies.VastOrderID);

            apiResponse.Success = true;
            apiResponse.Msg = "提交成功";
            apiLog.ResponseContent = apiResponse.Json();
            apiLog.Enter();
            return Json(apiResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新分拣那边的 ItemID
        /// </summary>
        private void UpdateSortingItemID(string batchID, Needs.Ccs.Services.Models.OrderItemPair[] newOrderItemPairs)
        {
            List<UpdateSortingItemResponseModel> listUpdateSortingItemResponseModel = new List<UpdateSortingItemResponseModel>();

            //找每一个 新的 OrderItemID 的 InputID
            if (newOrderItemPairs != null && newOrderItemPairs.Any())
            {
                foreach (var newOrderItemPair in newOrderItemPairs)
                {
                    if (newOrderItemPair.Inputs != null && newOrderItemPair.Inputs.Any())
                    {
                        foreach (var item in newOrderItemPair.Inputs)
                        {
                            listUpdateSortingItemResponseModel.Add(new UpdateSortingItemResponseModel()
                            {
                                InputID = item,
                                OrderItemID = newOrderItemPair.NewOrderItemID,
                                TinyOrderID = newOrderItemPair.TinyOrderID,
                            });
                        }
                    }
                }
            }


            foreach (var updateModel in listUpdateSortingItemResponseModel)
            {
                var requestModel = new
                {
                    inputID = updateModel.InputID,
                    itemID = updateModel.OrderItemID,
                    tinyOrderID = updateModel.TinyOrderID,
                };

                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.UpdateItem;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    Url = apiurl,
                    RequestContent = requestModel.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();

                try
                {
                    var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, requestModel);
                    apiLog.ResponseContent = result;
                    apiLog.Enter();
                }
                catch (Exception ex)
                {
                    ex.CcsLog("到货通知接口中调用库房更新ItemID接口");
                }
            }

        }

        /// <summary>
        /// 发送当前订单信息
        /// </summary>
        /// <param name="batchID"></param>
        /// <param name="mainOrderID"></param>
        /// <param name="oldOrderItemIDs"></param>
        /// <param name="newOrderItemPairs"></param>
        /// <param name="OrderItemInView返回代仓储"></param>
        /// <param name="OrderItemInLocal返回代仓储"></param>
        private void SendCurrentOrderInfo(string batchID, string mainOrderID,
            string[] oldOrderItemIDs, Needs.Ccs.Services.Models.OrderItemPair[] newOrderItemPairs,
            List<Needs.Ccs.Services.Views.DeliveriesTopModelOriginName> OrderItemInView返回代仓储,
            List<Needs.Ccs.Services.Models.OrderItem> OrderItemInLocal返回代仓储)
        {
            //按照代仓储格式填充 orderitem
            List<Needs.Ccs.Services.Views.OrderItemChanges> listOrderItemChanges = new List<Needs.Ccs.Services.Views.OrderItemChanges>();
            foreach (var item in OrderItemInView返回代仓储)
            {
                decimal unitPrice = 0;
                string unit = string.Empty;
                string customName = string.Empty;
                var oneOrderItemInLocal = OrderItemInLocal返回代仓储.Where(t => t.ID == item.IptItemID).FirstOrDefault();
                if (oneOrderItemInLocal != null)
                {
                    unitPrice = oneOrderItemInLocal.UnitPrice;
                    unit = oneOrderItemInLocal.Unit;
                    customName = oneOrderItemInLocal.Name;
                }

                listOrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                {
                    OrderItemID = item.IptItemID,
                    InputID = item.StoInputID,
                    CustomName = customName,
                    Product = new Needs.Ccs.Services.Views.CenterProduct()
                    {
                        PartNumber = item.PtvPartNumber,
                        Manufacturer = item.PtvManufacturer,
                    },
                    Origin = item.IptOrigin,
                    DateCode = item.IptDateCode,
                    Quantity = item.StoQuantity,
                    UnitPrice = unitPrice,
                    //TotalPrice = item.
                    Unit = unit,
                    TinyOrderID = item.IptTinyOrderID,
                });
            }

            //按照 pair 中，生成 inputID - newOrderItemID 对应关系
            Dictionary<string, string> dicInputNewOrderItemID = new Dictionary<string, string>();
            if (newOrderItemPairs != null && newOrderItemPairs.Any())
            {
                foreach (var newOrderItem in newOrderItemPairs)
                {
                    if (newOrderItem.Inputs != null && newOrderItem.Inputs.Any())
                    {
                        foreach (var inputID in newOrderItem.Inputs)
                        {
                            dicInputNewOrderItemID.Add(inputID, newOrderItem.NewOrderItemID);
                        }
                    }
                }
            }

            //按照 dic 中，将 对应 InputID 的 OrderItemID 换成新的
            for (int i = 0; i < listOrderItemChanges.Count; i++)
            {
                if (dicInputNewOrderItemID.ContainsKey(listOrderItemChanges[i].InputID))
                {
                    listOrderItemChanges[i].OrderItemID = dicInputNewOrderItemID[listOrderItemChanges[i].InputID];
                }
            }

            //获得大订单信息
            var mainOrderInfo = new Needs.Ccs.Services.Views.OrderItemsInMainOrderView().GetInfoByMainOrderID(mainOrderID);

            Needs.Ccs.Services.Views.CurrentOrderInfo currentOrderInfo = new Needs.Ccs.Services.Views.CurrentOrderInfo()
            {
                OrderID = mainOrderInfo.OrderID,
                Currency = mainOrderInfo.Currency,
                Confirmed = false,
                items = listOrderItemChanges,
            };

            var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitChanged;

            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = batchID,
                Url = apiurl,
                RequestContent = currentOrderInfo.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            apiLog.Enter();

            try
            {
                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, currentOrderInfo);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("到货通知接口中调用代仓储接口传当前到货信息");
            }
        }


        /// <summary>
        /// 到货信息 同步代仓储--20200414新
        /// </summary>
        /// <param name="batchID"></param>
        /// <param name="MainOrderID"></param>
        public void SendCurrentOrderInfoNew(string batchID, string MainOrderID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var deliverView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgDeliveriesTopView>();
                var orderitemView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();

                //本次到货
                var deliverList = deliverView.Where(t => t.OrderID == MainOrderID && t.TinyOrderID.Contains("-01")).ToList();
                //剩余应到
                var orderitemList = (from orderitem in orderitemView
                                     join order in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderitem.OrderID equals order.ID
                                     where order.MainOrderId == MainOrderID && order.DeclareFlag == (int)Needs.Ccs.Services.Enums.DeclareFlagEnums.Unable
                                     select new
                                     {
                                         items = orderitem
                                     }).ToList();

                //本次到货数量是否等于应到货数量
                if (deliverList.Sum(t => t.Quantity) != orderitemList.Sum(t => t.items.Quantity))
                {
                    return;
                }

                var modelinfo = from deliver in deliverView
                                join orderitem in orderitemView on deliver.ItemID equals orderitem.ID
                                where deliver.OrderID == MainOrderID
                                select new
                                {
                                    deliver,
                                    orderitem
                                };

                List<Needs.Ccs.Services.Views.OrderItemChanges> listOrderItemChanges = new List<Needs.Ccs.Services.Views.OrderItemChanges>();
                foreach (var item in modelinfo)
                {
                    listOrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                    {
                        OrderItemID = item.deliver.ItemID,
                        InputID = item.deliver.InputID,
                        CustomName = item.orderitem.Name,
                        Product = new Needs.Ccs.Services.Views.CenterProduct()
                        {
                            PartNumber = item.deliver.PartNumber,
                            Manufacturer = item.deliver.Manufacturer,
                        },
                        Origin = item.deliver.Origin,
                        DateCode = item.deliver.DateCode,
                        Quantity = item.deliver.Quantity,
                        UnitPrice = item.orderitem.UnitPrice,
                        TotalPrice = item.orderitem.TotalPrice,
                        Unit = item.orderitem.Unit,
                        TinyOrderID = item.deliver.TinyOrderID,
                    });
                }

                //获得大订单信息
                var mainOrderInfo = new Needs.Ccs.Services.Views.OrderItemsInMainOrderView().GetInfoByMainOrderID(MainOrderID);

                Needs.Ccs.Services.Views.CurrentOrderInfo currentOrderInfo = new Needs.Ccs.Services.Views.CurrentOrderInfo()
                {
                    OrderID = mainOrderInfo.OrderID,
                    Currency = mainOrderInfo.Currency,
                    Confirmed = false,
                    items = listOrderItemChanges,
                };

                var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitChanged;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    Url = apiurl,
                    RequestContent = currentOrderInfo.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "到货信息同步代仓储"
                };
                apiLog.Enter();

                try
                {
                    var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, currentOrderInfo);
                    apiLog.ResponseContent = result;
                    apiLog.Enter();
                }
                catch (Exception ex)
                {
                    ex.CcsLog("到货通知接口中调用代仓储接口传当前到货信息");
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ItemChangeRequest"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult ItemChangeNotice(JPost ItemChangeRequest)
        {
            try
            {
                //TODO:

                var ItemChange = ItemChangeRequest.ToObject<ItemChangeRequest>();
                //记录原始报文
                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    //OrderID = ItemChange.ID,
                    TinyOrderID = ItemChange.Notices.First()?.TinyOrderID,
                    RequestContent = ItemChange.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "库房推送产品变更"
                };
                apiLog.Enter();

                var view = new Needs.Ccs.Services.Views.AdminsTopView2();
                var npc = view.Where(t => t.OriginID == "XDTAdmin").FirstOrDefault();

                //修改订单项
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var OrderItemChangeNoticesView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>();

                    foreach (var t in ItemChange.Notices)
                    {
                        var admin = view.Where(item => item.ID == t.AdminID).FirstOrDefault();
                        if (admin == null)
                        {
                            admin = npc;
                        }

                        switch (t.Type)
                        {
                            case Needs.Ccs.Services.Enums.OrderItemChangeType.BrandChange:
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { Manufacturer = t.NewValue }, item => item.ID == t.ItemID);
                                var ifBrandPending = OrderItemChangeNoticesView.Where(item => item.OrderItemID == t.ItemID && item.Type == (int)t.Type && item.ProcessStatus == (int)Needs.Ccs.Services.Enums.ProcessState.UnProcess).FirstOrDefault();
                                if (ifBrandPending != null)
                                {
                                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                                                                        new { NewValue = t.NewValue, UpdateDate = DateTime.Now, AdminID = t.AdminID },
                                                                        item => item.ID == ifBrandPending.ID);
                                }
                                else
                                {
                                    InsertIntoOrderItemChange(t, admin);
                                }
                                break;

                            case Needs.Ccs.Services.Enums.OrderItemChangeType.OriginChange:
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { Origin = t.NewValue }, item => item.ID == t.ItemID);
                                var ifOriginPending = OrderItemChangeNoticesView.Where(item => item.OrderItemID == t.ItemID && item.Type == (int)t.Type && item.ProcessStatus == (int)Needs.Ccs.Services.Enums.ProcessState.UnProcess).FirstOrDefault();
                                if (ifOriginPending != null)
                                {
                                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                                                                        new { NewValue = t.NewValue, UpdateDate = DateTime.Now, AdminID = t.AdminID },
                                                                        item => item.ID == ifOriginPending.ID);
                                }
                                else
                                {
                                    InsertIntoOrderItemChange(t, admin);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }


                //将变更信息发送给客户端(所有订单信息)
                string tinyOrderID = ItemChange.Notices.First().TinyOrderID;
                var OriginOrder = new Needs.Ccs.Services.Views.OrdersView().Where(t => t.ID == tinyOrderID).FirstOrDefault();
                ItemChangePost2AgentWarehouse(OriginOrder.MainOrderID, OriginOrder);


                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("库房推送产品变更失败");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void InsertIntoOrderItemChange(ItemChangeNotices item, Needs.Ccs.Services.Models.Admin admin)
        {
            Needs.Ccs.Services.Models.OrderItemChangeNotice orderItemChangeNotice = new Needs.Ccs.Services.Models.OrderItemChangeNotice();
            orderItemChangeNotice.Type = item.Type;
            orderItemChangeNotice.TriggerSource = Needs.Ccs.Services.Enums.TriggerSource.CheckDecListMan;
            orderItemChangeNotice.OrderItemID = item.ItemID;
            orderItemChangeNotice.ProcessState = Needs.Ccs.Services.Enums.ProcessState.UnProcess;
            orderItemChangeNotice.Status = Needs.Ccs.Services.Enums.Status.Normal;
            orderItemChangeNotice.CreateDate = DateTime.Now;
            orderItemChangeNotice.UpdateDate = DateTime.Now;
            orderItemChangeNotice.OldValue = item.OldValue;
            orderItemChangeNotice.NewValue = item.NewValue;
            orderItemChangeNotice.IsSplited = false;
            orderItemChangeNotice.Sorter = admin;

            orderItemChangeNotice.Enter();

            Needs.Ccs.Services.Models.OrderItemChangeLog orderItemChangeLog = new Needs.Ccs.Services.Models.OrderItemChangeLog();
            orderItemChangeLog.OrderID = item.TinyOrderID;
            orderItemChangeLog.OrderItemID = item.ItemID;
            orderItemChangeLog.Admin = admin;
            orderItemChangeLog.Type = item.Type;
            orderItemChangeLog.Summary = "库房[" + admin.RealName + "]做了" + item.Type.GetDescription() + "操作,从[" + item.OldValue + "]变更为[" + item.NewValue + "]";
            orderItemChangeLog.Enter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpiltRequest"></param>
        /// <returns></returns>
        public ActionResult SplitChangeNotice(JPost SpiltRequest)
        {
            try
            {
                //TODO:
                var ItemChange = SpiltRequest.ToObject<SplitChangeRequest>();
                //记录原始报文
                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    //OrderID = ItemChange.ID,
                    TinyOrderID = ItemChange.Notices.First()?.TinyOrderID,
                    RequestContent = ItemChange.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "库房推送拆项变更"
                };
                apiLog.Enter();

                string tinyOrderID = ItemChange.Notices.First().TinyOrderID;
                var OriginOrder = new Needs.Ccs.Services.Views.OrdersView().Where(t => t.ID == tinyOrderID).FirstOrDefault();

                List<Needs.Ccs.Services.Models.OrderItemAssitant> OrderItems = new List<Needs.Ccs.Services.Models.OrderItemAssitant>();
                List<ItemChangeNotices> ProductChangeNotices = new List<ItemChangeNotices>();
                List<string> SplitOrderItemInfo = new List<string>();

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orderItemView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
                    //验证
                    foreach (var t in ItemChange.Notices)
                    {
                        var orderItem = orderItemView.Where(item => item.ID == t.ItemID).FirstOrDefault();
                        if (orderItem != null)
                        {
                            if (t.Qty >= orderItem.Quantity)
                            {
                                var jsonqty = new JMessage() { code = 400, success = false, data = "拆项数量不能大于原始订单数量" };
                                return Json(jsonqty, JsonRequestBehavior.AllowGet);
                            }
                            //else
                            //{
                            //    var sameItem = orderItemView.Where(item => item.OrderID == orderItem.OrderID && item.Origin == t.Origin && item.Manufacturer == t.Brand && item.Model == orderItem.Model).FirstOrDefault();
                            //    if (sameItem != null)
                            //    {
                            //        var jsonqty = new JMessage() { code = 400, success = false, data = "有型号，品牌，产地一致的订单项,不允许拆分" };
                            //        return Json(jsonqty, JsonRequestBehavior.AllowGet);
                            //    }
                            //}
                        }                        
                    }
                    

                    //插入新的订单项，修改原订单项数量
                    foreach(var t in ItemChange.Notices)
                    {
                        var orderItemOld = orderItemView.Where(item => item.ID == t.ItemID).FirstOrDefault();
                        var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                        OrderItem newOrderItem = new OrderItem();
                        string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);                        
                        newOrderItem.ID = singleOrderItemID;
                        newOrderItem.OrderID = orderItemOld.OrderID;
                        newOrderItem.Origin = t.Origin;
                        newOrderItem.Quantity = t.Qty;
                        newOrderItem.Unit = orderItemOld.Unit;
                        newOrderItem.UnitPrice = orderItemOld.UnitPrice;
                        newOrderItem.TotalPrice = Math.Round(orderItemOld.UnitPrice*t.Qty,2,MidpointRounding.AwayFromZero);
                        newOrderItem.IsSampllingCheck = false;
                        newOrderItem.ClassifyStatus = Needs.Ccs.Services.Enums.ClassifyStatus.Unclassified;
                        newOrderItem.Status = Needs.Ccs.Services.Enums.Status.Normal;
                        newOrderItem.CreateDate = DateTime.Now;
                        newOrderItem.UpdateDate = DateTime.Now;
                        newOrderItem.Name = orderItemOld.Name;
                        newOrderItem.Model = orderItemOld.Model;
                        newOrderItem.Manufacturer = t.Brand;
                        newOrderItem.Batch = orderItemOld.Batch;
                        reponsitory.Insert(newOrderItem.ToLinq());

                        //修改原订单项数量,和订单项的总价
                        var OrderItemOldQty = orderItemOld.Quantity - t.Qty;
                        var OrderItemOldPrice = Math.Round((OrderItemOldQty * orderItemOld.UnitPrice),2,MidpointRounding.AwayFromZero);
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { Quantity = OrderItemOldQty, TotalPrice= OrderItemOldPrice }, item => item.ID == t.ItemID);
                        SplitOrderItemInfo.Add(t.ItemID);
                        Needs.Ccs.Services.Models.OrderItemAssitant orderItemAssitant = new OrderItemAssitant();
                        orderItemAssitant.ID = singleOrderItemID;
                        orderItemAssitant.TotalPrice = newOrderItem.TotalPrice;
                        orderItemAssitant.InputID = t.InputID;
                        orderItemAssitant.UnitPrice = orderItemOld.UnitPrice;
                        orderItemAssitant.ChangeType = Needs.Ccs.Services.Enums.MatchChangeType.ProductChange;
                        orderItemAssitant.PersistenceType = Needs.Ccs.Services.Enums.PersistenceType.Insert;
                        orderItemAssitant.MatchedOrderItemID = t.ItemID;
                        OrderItems.Add(orderItemAssitant);
                        
                        if(t.Origin!= orderItemOld.Origin)
                        {
                            ItemChangeNotices productChange = new ItemChangeNotices();
                            productChange.ItemID = singleOrderItemID;
                            productChange.NewValue = t.Origin;
                            productChange.OldValue = orderItemOld.Origin;
                            productChange.Type = Needs.Ccs.Services.Enums.OrderItemChangeType.OriginChange;
                            productChange.AdminID = t.AdminID;
                            productChange.TinyOrderID = t.TinyOrderID;
                            ProductChangeNotices.Add(productChange);
                        }

                        if(t.Brand!= orderItemOld.Manufacturer)
                        {
                            ItemChangeNotices productChange = new ItemChangeNotices();
                            productChange.ItemID = singleOrderItemID;
                            productChange.NewValue = t.Brand;
                            productChange.OldValue = orderItemOld.Manufacturer;
                            productChange.Type = Needs.Ccs.Services.Enums.OrderItemChangeType.BrandChange;
                            productChange.AdminID = t.AdminID;
                            productChange.TinyOrderID = t.TinyOrderID;
                            ProductChangeNotices.Add(productChange);
                        }                      
                    }

                    
                }

                //复制归类信息
                CopyClassifyResult(OrderItems, OriginOrder, tinyOrderID);

                ////这里都是新生成的OrderItem 所以不用判断之前是否有这个OrderItem的产品变更
                ////生成产品变更，不需要生成订单变更（重新归类的时候会生成相应的订单变更）
                var view = new Needs.Ccs.Services.Views.AdminsTopView2();
                var npc = view.Where(t => t.OriginID == "XDTAdmin").FirstOrDefault();
                foreach (var t in ProductChangeNotices)
                {
                    var admin = view.Where(item => item.ID == t.AdminID).FirstOrDefault();
                    if (admin == null)
                    {
                        admin = npc;
                    }
                    InsertIntoOrderItemChange(t, admin);
                }

                //将Input信息发送给库房
                Post2WareHouse(OrderItems, OriginOrder);
                //将变更信息发送给客户端(所有订单信息)
                SplitPost2AgentWarehouse(OriginOrder.MainOrderID, OriginOrder, SplitOrderItemInfo);
                //同步归类信息
                SyncClassifyResult(OrderItems);

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                ex.CcsLog("库房推送产品变更失败");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }            
        }

        private void CopyClassifyResult(List<Needs.Ccs.Services.Models.OrderItemAssitant> orderItems, Order originOrder, string currentOrderID)
        {
            MatchCopyClassifyResult copyClassifyResult = new MatchCopyClassifyResult(orderItems, originOrder, currentOrderID);
            copyClassifyResult.Copy();

           
        }

        private void SyncClassifyResult(List<Needs.Ccs.Services.Models.OrderItemAssitant> orderItems)
        {
            MatchSyncClassify matchSyncClassify = new MatchSyncClassify(orderItems);
            matchSyncClassify.SyncResult();
        }

        private void ItemChangePost2AgentWarehouse(string currentMainOrderID, Order currentOrder)
        {
            MatchPost2AgentWarehouse post = new MatchPost2AgentWarehouse(currentMainOrderID, currentOrder);
            post.ItemChangePost();
        }

        private void SplitPost2AgentWarehouse(string currentMainOrderID, Order currentOrder,List<string> splitOrderItemInfos)
        {
            MatchPost2AgentWarehouse post = new MatchPost2AgentWarehouse(currentMainOrderID, currentOrder, splitOrderItemInfos);
            post.Post();
        }



        private void Post2WareHouse(List<Needs.Ccs.Services.Models.OrderItemAssitant> orderItems, Order originOrder)
        {
            MatchPost2Warehouse post2Warehouse = new MatchPost2Warehouse(orderItems, originOrder);
            post2Warehouse.Post();
        }
    }
}
