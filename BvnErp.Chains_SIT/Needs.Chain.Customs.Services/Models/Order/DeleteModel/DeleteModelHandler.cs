using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeleteModelHandler
    {
        /// <summary>
        /// 特殊类型对应关系（型号 - 订单）
        /// </summary>
        private readonly Dictionary<Enums.ItemCategoryType, Enums.OrderSpecialType> SpecialTypeRelation_Item_Order = new Dictionary<Enums.ItemCategoryType, Enums.OrderSpecialType>
        {
            { Enums.ItemCategoryType.HighValue, Enums.OrderSpecialType.HighValue },
            { Enums.ItemCategoryType.Inspection, Enums.OrderSpecialType.Inspection },
            { Enums.ItemCategoryType.Quarantine, Enums.OrderSpecialType.Quarantine },
            { Enums.ItemCategoryType.CCC, Enums.OrderSpecialType.CCC },
        };

        /// <summary>
        /// 特殊类型对应关系（型号 - 报关单）
        /// </summary>
        private readonly Dictionary<Enums.ItemCategoryType, Enums.DecHeadSpecialTypeEnum> SpecialTypeRelation_Item_DecHead = new Dictionary<Enums.ItemCategoryType, Enums.DecHeadSpecialTypeEnum>
        {
            { Enums.ItemCategoryType.HighValue, Enums.DecHeadSpecialTypeEnum.HighValue },
            { Enums.ItemCategoryType.Inspection, Enums.DecHeadSpecialTypeEnum.Inspection },
            { Enums.ItemCategoryType.Quarantine, Enums.DecHeadSpecialTypeEnum.Quarantine },
            { Enums.ItemCategoryType.CCC, Enums.DecHeadSpecialTypeEnum.CCC },
        };

        private string OrderItemID { get; set; } = string.Empty;

        private string OrderID { get; set; } = string.Empty;

        private List<Enums.ItemCategoryType> ThisItemCategoryTypes { get; set; } = new List<Enums.ItemCategoryType>();

        public string Msg { get; set; } = string.Empty;

        public Enums.OrderStatus OrderStatus { get; set; }

        public Enums.OrderType OrderType { get; set; }

        public DeleteModelHandler(string orderItemID, string orderID)
        {
            this.OrderItemID = orderItemID;
            this.OrderID = orderID;
        }

        public bool Execute()
        {
            try
            {
                string sortingID = string.Empty;
                Enums.OrderType orderType;
                Enums.OrderStatus orderStatus;  //根据订单状态判断是否挂起订单（>= QuoteConfirmed  4  可挂起订单）

                bool isHasHkSortings = false; //用这个判断这个型号是否装箱

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    //查询状态为 200 的该订单 Order, 如果查不到该订单，则返回错误
                    //如果查询到，则记录下订单类型（内单/外单）
                    var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        .Where(t => t.ID == this.OrderID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                    if (order == null)
                    {
                        this.Msg = "不存在 OrderID = " + this.OrderID + " 的订单";
                        return false;
                    }
                    orderType = (Enums.OrderType)order.Type;
                    this.OrderType = (Enums.OrderType)order.Type;
                    orderStatus = (Enums.OrderStatus)order.OrderStatus;
                    this.OrderStatus = (Enums.OrderStatus)order.OrderStatus;
                    if (orderStatus == Enums.OrderStatus.Draft)
                    {
                        this.Msg = "订单 " + this.OrderID + " 为草稿状态，请到订单草稿中操作";
                        return false;
                    }
                    //else if (orderStatus == Enums.OrderStatus.Confirmed)
                    //{
                    //    this.Msg = "订单 " + this.OrderID + " 为待归类状态，暂不支持删除型号";
                    //    return false;
                    //}

                    //查询该订单下状态为 200 的 OrderItem 数量，如果 <= 1 则提示不能删除
                    var orderItemCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                        .Where(t => t.OrderID == this.OrderID
                                 && t.Status == (int)Enums.Status.Normal).Count();
                    if (orderItemCount <= 1)
                    {
                        this.Msg = "订单 " + this.OrderID + " 中只有一个型号，不能删除";
                        return false;
                    }

                    //外单查询出型号信息，用于拼接挂起的原因，需要展示给客户确认时看
                    var orderItemInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.ID == this.OrderItemID)
                        .Select(t => new Models.OrderItem
                        {
                            ID = t.ID,
                            Model = t.Model,
                            Manufacturer = t.Manufacturer,
                            Quantity = t.Quantity,
                        }).FirstOrDefault();
                    if (orderItemInfo == null)
                    {
                        this.Msg = "不存在 OrderItemID = " + this.OrderItemID + " 的型号";
                        return false;
                    }

                    //查询状态为 200 的香港库房的 该 OrderItem 的 Sorting
                    //记录下是否有该型号的 香港库房的 Sorting (isHasHkSortings) 和 sortingID
                    var sorting = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()
                        .Where(t => t.OrderItemID == this.OrderItemID
                                 && t.WarehouseType == (int)Enums.WarehouseType.HongKong
                                 && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                    if (sorting != null)
                    {
                        isHasHkSortings = true;
                        sortingID = sorting.ID;
                    }



                    //查出 OrderControl 表、OrderControlStep 表中与该订单相关的数据(这里将 200/400 的都查询出来了)
                    Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls = null;
                    Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps = null;

                    int[] approveOrderControlTypeInt = 
                    {
                        (int)Enums.OrderControlType.GenerateBillApproval,
                        (int)Enums.OrderControlType.DeleteModelApproval,
                        (int)Enums.OrderControlType.ChangeQuantityApproval,
                        (int)Enums.OrderControlType.SplitOrderApproval
                    };

                    oldOrderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(
                        t => t.OrderID == this.OrderID && !approveOrderControlTypeInt.Contains(t.ControlType)).ToArray();
                    if (oldOrderControls != null && oldOrderControls.Any())
                    {
                        string[] oldOrderControlIDs = oldOrderControls.Select(t => t.ID).ToArray();
                        oldOrderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => oldOrderControlIDs.Contains(t.OrderControlID)).ToArray();
                    }

                    //查出该型号的类型，用于删除后，订单特殊类型和报关单特殊类型的判断
                    var itemSpecialTypes1 = SpecialTypeRelation_Item_Order.Keys.ToList();
                    var itemSpecialTypes2 = SpecialTypeRelation_Item_DecHead.Keys.ToList();

                    var allItemSpecialTypes = itemSpecialTypes1.Union(itemSpecialTypes2).ToArray();


                    var orderItemCategory = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(t => t.OrderItemID == this.OrderItemID).FirstOrDefault();
                    if (orderItemCategory != null)
                    {
                        //this.ThisItemCategoryType = (Enums.ItemCategoryType)orderItemCategory.Type;
                        foreach (var preItemSpecialType in allItemSpecialTypes)
                        {
                            if ((orderItemCategory.Type & (int)preItemSpecialType) == (int)preItemSpecialType)
                            {
                                this.ThisItemCategoryTypes.Add(preItemSpecialType);
                            }
                        }
                    }






                    DeleteModelDo deleteModelDo = new OutsideDeleteModel(reponsitory, this.OrderItemID, sortingID, this.OrderID, orderStatus, orderItemInfo, oldOrderControls, oldOrderControlSteps);
                    if (orderType == Enums.OrderType.Outside)
                    {
                        Enums.EntryNoticeStatus hkEntryNoticeStatus = Enums.EntryNoticeStatus.UnBoxed;

                        //香港入库通知若存在，则记下 香港入库通知的状态，在外单中需要判断是否【封箱】
                        var hkEntryNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                            .Where(t => t.OrderID == this.OrderID
                                     && t.WarehouseType == (int)Enums.WarehouseType.HongKong
                                     && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                        if (hkEntryNotice != null)
                        {
                            hkEntryNoticeStatus = (Enums.EntryNoticeStatus)hkEntryNotice.EntryNoticeStatus;
                        }

                        //外单已封箱的话，不能删除型号
                        if (hkEntryNoticeStatus == Enums.EntryNoticeStatus.Sealed)
                        {
                            this.Msg = "该订单已封箱，不能删除型号";
                            return false;
                        }

                        //通过是否有香港的 Sorting 判断这个型号是否已装箱
                        if (isHasHkSortings)
                        {
                            this.Msg = "该型号已装箱，不能删除型号";
                            return false;
                        }

                        deleteModelDo = new OutsideDeleteModel(reponsitory, this.OrderItemID, sortingID, this.OrderID, orderStatus, orderItemInfo, oldOrderControls, oldOrderControlSteps);
                    }
                    else if (orderType == Enums.OrderType.Icgoo || orderType == Enums.OrderType.Inside)
                    {
                        var decHead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                            .Where(t => t.OrderID == this.OrderID).FirstOrDefault();
                        if (decHead != null)
                        {
                            //如果报关完成，则不让删除型号
                            if (decHead.IsSuccess)
                            {
                                this.Msg = "该订单已报关完成，不能删除型号";
                                return false;
                            }
                        }

                        deleteModelDo = new InsideDeleteModel(reponsitory, this.OrderItemID, sortingID, this.OrderID, oldOrderControls, oldOrderControlSteps);
                    }
                    else
                    {
                        this.Msg = "订单类型不是一个已知的类型";
                        return false;
                    }

                    deleteModelDo.Do();

                    //将原“代理报关委托书”置为 400
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderFiles>(new
                    {
                        Status = (int)Enums.Status.Delete,
                    }, item => item.OrderID == this.OrderID
                    && item.FileType == (int)Enums.FileType.AgentTrustInstrument
                    && item.Status == (int)Enums.Status.Normal);

                    //删除与该型号相关的、未处理的香港库房产生的型号变更通知
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                        item => item.OrderItemID == this.OrderItemID
                             && item.ProcessStatus != (int)Enums.ProcessState.Processed);

                    reponsitory.Submit();
                }

                //整理该订单的订单特殊类型和报关单特殊类型
                //删除的型号具有某种特殊类型，则执行检查，否则不动作
                //检查该订单中，是否还有具有该特殊类型的型号，如果没有；则执行删除特殊类型，如果还有该特殊类型的型号，则不动作


                //if (allItemSpecialTypes.Contains(this.ThisItemCategoryType))
                //{
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orderItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
                    var orderItemCategories = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

                    var currentCategoryInfoInThisOrder = (from orderItem in orderItems
                                                          join orderItemCategory in orderItemCategories
                                                          on new
                                                          {
                                                              OrderItemID = orderItem.ID,
                                                              OrderItemDataStatus = orderItem.Status,
                                                              OrderItemCategoryDataStatus = (int)Enums.Status.Normal,
                                                              OrderID = orderItem.OrderID,
                                                          }
                                                          equals new
                                                          {
                                                              OrderItemID = orderItemCategory.OrderItemID,
                                                              OrderItemDataStatus = (int)Enums.Status.Normal,
                                                              OrderItemCategoryDataStatus = orderItemCategory.Status,
                                                              OrderID = this.OrderID,
                                                          }
                                                          select orderItemCategory).ToList();

                    var itemSpecialTypes1 = SpecialTypeRelation_Item_Order.Keys.ToList();
                    var itemSpecialTypes2 = SpecialTypeRelation_Item_DecHead.Keys.ToList();

                    var allItemSpecialTypes = itemSpecialTypes1.Union(itemSpecialTypes2).ToArray();

                    foreach (var itemThisItemCategoryType in this.ThisItemCategoryTypes)
                    {
                        if (allItemSpecialTypes.Contains(itemThisItemCategoryType))
                        {

                            bool isHasThisSpecialTypeItem = false;

                            if (currentCategoryInfoInThisOrder != null && currentCategoryInfoInThisOrder.Any())
                            {
                                foreach (var category in currentCategoryInfoInThisOrder)
                                {
                                    if ((category.Type & (int)itemThisItemCategoryType) == (int)itemThisItemCategoryType)
                                    {
                                        isHasThisSpecialTypeItem = true;
                                        break;
                                    }
                                }
                            }

                            if (!isHasThisSpecialTypeItem)
                            {
                                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderVoyages>(
                                    item => item.OrderID == this.OrderID
                                            && item.Type == (int)SpecialTypeRelation_Item_Order[itemThisItemCategoryType]);

                                var decHead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.OrderID == this.OrderID).FirstOrDefault();
                                if (decHead != null)
                                {
                                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecHeadSpecialTypes>(
                                        item => item.DecHeadID == decHead.ID
                                                && item.Type == (int)SpecialTypeRelation_Item_DecHead[itemThisItemCategoryType]);
                                }
                            }
                        }
                    }


                    //如果是内单，当没有未处理的管控时，将订单状态修改为“待报关”
                    if (orderType == Enums.OrderType.Icgoo || orderType == Enums.OrderType.Inside)
                    {
                        var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                        var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                        var unAuditedControlsCount = (from orderControl in orderControls
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
                                                                OrderID = this.OrderID,
                                                                ControlStatus = orderControlStep.ControlStatus,
                                                            }
                                                      select new OrderControlData
                                                      {
                                                          ID = orderControl.ID,
                                                      }).Count();

                        if (unAuditedControlsCount <= 0)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                            {
                                OrderStatus = (int)Enums.OrderStatus.QuoteConfirmed,
                            }, item => item.ID == this.OrderID);
                        }

                    }



                }
                //}

                //将原先 超垫款上限管控 相关信息都删除
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
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
                                                                  OrderID = this.OrderID,
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


            }
            catch (Exception ex)
            {

                throw new Exception("DeleteModel 内执行错误：" + ex.Message);
            }


            return true;
        }


        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="orderType"></param>
        public void UpdateOrder(Admin admin, Enums.OrderType orderType)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderID = this.OrderID;
                var declarant = admin;

                //未完成归类的产品数量
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Count(item => item.OrderID == orderID &&
                        (item.ClassifyStatus == (int)Enums.ClassifyStatus.Unclassified || item.ClassifyStatus == (int)Enums.ClassifyStatus.First) &&
                        item.Status == (int)Enums.Status.Normal);

                if (count == 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                    {
                        OrderStatus = (int)Enums.OrderStatus.Classified
                    }, item => item.ID == orderID);

                    var order = new Views.OrdersView(reponsitory)[orderID];

                    if (declarant != null)
                    {
                        order?.Log(declarant, "跟单员【" + declarant.RealName + "】因删除未归类的最后一个型号，完成了订单归类，等待跟单员报价");
                        order.Trace(declarant, Enums.OrderTraceStep.Processing, "您的订单产品已归类完成");
                    }

                    //如果代理费类型为指定费用, 查出这个指定费用 Begin
                    decimal PointedAgencyFee = 0;
                    if (order.OrderBillType == Enums.OrderBillType.Pointed)
                    {
                        var agency = new Needs.Ccs.Services.Views.Origins.OrderPremiumsOrigin().Where(
                                                                    t => t.OrderID == order.ID
                                                                        && t.Type == Enums.OrderPremiumType.AgencyFee
                                                                        && t.Status == Enums.Status.Normal).FirstOrDefault();
                        if (agency != null)
                        {
                            PointedAgencyFee = agency.UnitPrice;
                        }
                    }
                    //如果代理费类型为指定费用, 查出这个指定费用 End

                    //Icgoo和大赢家订单归类完成，系统自动完成报价和确认报价，进入待报关状态
                    if (orderType != Enums.OrderType.Outside)
                    {
                        try
                        {
                            order.GenerateBill(order.OrderBillType, PointedAgencyFee);

                            var classifiedOrder = new Views.ClassifiedOrdersView(reponsitory)[orderID];
                            classifiedOrder?.Quote();

                            var quotedOrder = new Views.QuotedOrdersView(reponsitory)[orderID];
                            quotedOrder?.IcgooQuoteConfirm();
                            quotedOrder?.ToReceivables();
                        }
                        catch (Exception ex)
                        {

                            throw new Exception("内单重新生成对账单，错误：");
                        }
                    }
                    else
                    {
                        try
                        {
                            //var order = new Views.OrdersView()[orderID];
                            var bill = order.Files.Where(file => file.FileType == Enums.FileType.OrderBill && file.Status == Enums.Status.Normal).SingleOrDefault();

                            bill?.Abandon();
                            order.GenerateBill(order.OrderBillType, PointedAgencyFee);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("外单重新生成对账单，错误：" + ex.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        //var classifiedOrder = new Views.ClassifiedOrdersView(reponsitory)[orderID];
                        //classifiedOrder?.GenerateBill(); //为了更新报关总货值等

                        var order = new Views.OrdersView(reponsitory)[orderID];
                        var bill = order.Files.Where(file => file.FileType == Enums.FileType.OrderBill && file.Status == Enums.Status.Normal).SingleOrDefault();

                        //如果代理费类型为指定费用, 查出这个指定费用 Begin
                        decimal PointedAgencyFee = 0;
                        if (order.OrderBillType == Enums.OrderBillType.Pointed)
                        {
                            var agency = new Needs.Ccs.Services.Views.Origins.OrderPremiumsOrigin().Where(
                                                                        t => t.OrderID == order.ID
                                                                            && t.Type == Enums.OrderPremiumType.AgencyFee
                                                                            && t.Status == Enums.Status.Normal).FirstOrDefault();
                            if (agency != null)
                            {
                                PointedAgencyFee = agency.UnitPrice;
                            }
                        }
                        //如果代理费类型为指定费用, 查出这个指定费用 End

                        bill?.Abandon();
                        order.GenerateBill(order.OrderBillType, PointedAgencyFee);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception("更新报关总货值等，错误：" + ex.Message);
                    }


                }

            }
        }


        /// <summary>
        /// 调用代仓储前端接口，删除型号
        /// </summary>
        /// <param name="admin"></param>
        public void CallPvWsOrderConfirm(Admin admin)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == this.OrderID).FirstOrDefault();

                var confirm = new ClientConfirm();
                confirm.OrderID = order.MainOrderId;
                confirm.AdminID = admin.ErmAdminID;
                confirm.OrderItemID = this.OrderItemID;
                confirm.Type = ConfirmType.DeleteItem;

                var apisetting = new ApiSettings.PvWsOrderApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ClientConfirm;
                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, confirm);
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);

                if (message.code != 200)
                {
                    OrderLog log = new OrderLog();
                    log.OrderID = order.ID;
                    log.Admin = admin;
                    log.OrderStatus = (Enums.OrderStatus)order.OrderStatus;
                    log.Summary = "订单删除型号推送代仓储失败:" + message.data;
                    log.Enter();
                }
            }
        }
    }

    public abstract class DeleteModelDo
    {
        public abstract void Do();

        /// <summary>
        /// 删除与该 orderItem 有关的 OrderControl、OrderControlStep 信息
        /// </summary>
        public void AbandonThisOrderItemControl(Layer.Data.Sqls.ScCustomsReponsitory reponsitory,
            string orderItemID,
            string orderID,
            Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls,
            Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps)
        {
            if (oldOrderControls == null || !oldOrderControls.Any())
            {
                return;
            }

            string[] thisOrderItemOrderControlIDs = oldOrderControls.Where(t => t.OrderItemID == orderItemID).Select(t => t.ID).ToArray();
            reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(item => thisOrderItemOrderControlIDs.Contains(item.OrderControlID));
            reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderControls>(item => thisOrderItemOrderControlIDs.Contains(item.ID));


            var stepNoThisItem = oldOrderControlSteps.Where(t => !thisOrderItemOrderControlIDs.Contains(t.OrderControlID)).ToArray();
            var controlNoThisItem = oldOrderControls.Where(t => !thisOrderItemOrderControlIDs.Contains(t.ID)).ToArray();

            //得到与该型号无关的未处理过的 control
            var unHandledControl = (from control in controlNoThisItem
                                    join step in stepNoThisItem on control.ID equals step.OrderControlID
                                    where control.Status == (int)Enums.Status.Normal
                                       && step.Status == (int)Enums.Status.Normal
                                       && step.ControlStatus == (int)Enums.OrderControlStatus.Auditing  //如果是拒绝通过，会取消挂起订单吗？
                                    select control).ToArray();

            if (unHandledControl == null || !unHandledControl.Any())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    IsHangUp = false,
                }, item => item.ID == orderID);
            }
        }
    }

}
