using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ArrivalDeleteModelHandler
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

        private List<string> OrderItemIDs { get; set; }

        private string OrderID { get; set; } = string.Empty;

        private List<Enums.ItemCategoryType> ThisItemCategoryTypes { get; set; } = new List<Enums.ItemCategoryType>();

        public string Msg { get; set; } = string.Empty;

        public Enums.OrderStatus OrderStatus { get; set; }

        public Enums.OrderType OrderType { get; set; }

        public ArrivalDeleteModelHandler(List<string> orderItemIDs, string orderID)
        {
            this.OrderItemIDs = orderItemIDs;
            this.OrderID = orderID;
        }

        public bool Execute()
        {
            try
            {
                string sortingID = string.Empty;
                Enums.OrderType orderType;
                Enums.OrderStatus orderStatus;  //根据订单状态判断是否挂起订单（>= QuoteConfirmed  4  可挂起订单）

                foreach (var SingleOrderItemID in this.OrderItemIDs)
                {
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                    {
                        var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                            .Where(t => t.ID == this.OrderID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();

                        orderType = (Enums.OrderType)order.Type;
                        this.OrderType = (Enums.OrderType)order.Type;
                        orderStatus = (Enums.OrderStatus)order.OrderStatus;
                        this.OrderStatus = (Enums.OrderStatus)order.OrderStatus;

                        int[] approveOrderControlTypeInt =
                           {
                            (int)Enums.OrderControlType.GenerateBillApproval,
                            (int)Enums.OrderControlType.DeleteModelApproval,
                            (int)Enums.OrderControlType.ChangeQuantityApproval,
                            (int)Enums.OrderControlType.SplitOrderApproval
                        };

                        //查出 OrderControl 表、OrderControlStep 表中与该订单相关的数据(这里将 200/400 的都查询出来了)
                        Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls = null;
                        Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps = null;

                        oldOrderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(
                            t => t.OrderID == this.OrderID && !approveOrderControlTypeInt.Contains(t.ControlType)).ToArray();
                        if (oldOrderControls != null && oldOrderControls.Any())
                        {
                            string[] oldOrderControlIDs = oldOrderControls.Select(t => t.ID).ToArray();
                            oldOrderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => oldOrderControlIDs.Contains(t.OrderControlID)).ToArray();
                        }

                        //查出该型号的类型，用于删除后，订单特殊类型和报关单特殊类型的判断
                        var itemSpecialTypes1 = SpecialTypeRelation_Item_Order.Keys.ToList();

                        var allItemSpecialTypes = itemSpecialTypes1.ToArray();



                        var orderItemInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.ID == SingleOrderItemID)
                       .Select(t => new Models.OrderItem
                       {
                           ID = t.ID,
                           Model = t.Model,
                           Manufacturer = t.Manufacturer,
                           Quantity = t.Quantity,
                       }).FirstOrDefault();

                        var orderItemCategory = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(t => t.OrderItemID == SingleOrderItemID).FirstOrDefault();
                        if (orderItemCategory != null)
                        {
                            foreach (var preItemSpecialType in allItemSpecialTypes)
                            {
                                if ((orderItemCategory.Type & (int)preItemSpecialType) == (int)preItemSpecialType)
                                {
                                    this.ThisItemCategoryTypes.Add(preItemSpecialType);
                                }
                            }
                        }

                        DeleteModelDo deleteModelDo = new ArrivalOutsideDeleteModel(reponsitory, SingleOrderItemID, sortingID, this.OrderID, orderStatus, orderItemInfo, oldOrderControls, oldOrderControlSteps);
                        if (orderType == Enums.OrderType.Outside)
                        {
                            deleteModelDo = new ArrivalOutsideDeleteModel(reponsitory, SingleOrderItemID, sortingID, this.OrderID, orderStatus, orderItemInfo, oldOrderControls, oldOrderControlSteps);
                        }
                        else
                        {
                            this.Msg = "订单类型不是一个已知的类型";
                            return false;
                        }
                        deleteModelDo.Do();

                        //删除与该型号相关的、未处理的香港库房产生的型号变更通知
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>(
                            item => item.OrderItemID == SingleOrderItemID
                                 && item.ProcessStatus != (int)Enums.ProcessState.Processed);



                        //将原“代理报关委托书”置为 400
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(new
                        {
                            Status = (int)Enums.Status.Delete,
                        }, item => item.MainOrderID == order.MainOrderId
                        && item.FileType == (int)Enums.FileType.AgentTrustInstrument
                        && item.Status == (int)Enums.Status.Normal);

                        reponsitory.Submit();
                    }
                }
                    

                //整理该订单的订单特殊类型和报关单特殊类型
                //删除的型号具有某种特殊类型，则执行检查，否则不动作
                //检查该订单中，是否还有具有该特殊类型的型号，如果没有；则执行删除特殊类型，如果还有该特殊类型的型号，则不动作

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

                    var allItemSpecialTypes = itemSpecialTypes1.ToArray();

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
                            }
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
                //confirm.OrderItemID = this.OrderItemID;
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
}
