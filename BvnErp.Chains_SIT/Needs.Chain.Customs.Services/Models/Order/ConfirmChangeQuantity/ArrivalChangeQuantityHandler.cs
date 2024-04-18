using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ArrivalChangeQuantityHandler
    {
        private List<OrderItemChangeCauseOrderChange> OrderItemInfos { get; set; }

        private string OrderID { get; set; } = string.Empty;

        public string Msg { get; set; } = string.Empty;

        public Enums.OrderStatus OrderStatus { get; set; }
        public ArrivalChangeQuantityHandler(List<OrderItemChangeCauseOrderChange> orderItemInfos, string orderID)
        {
            this.OrderItemInfos = orderItemInfos;
            this.OrderID = orderID;
        }

        public bool Execute()
        {
            try
            {
                decimal orderItemUnitPrice = 0;
                Enums.OrderType orderType;
                Enums.OrderStatus orderStatus;  //根据订单状态判断是否挂起订单（>= QuoteConfirmed  4  可挂起订单）

                string sortingIDForInside = string.Empty;


                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    //检查 Order 中是否存在，若存在，记下 订单类型
                    var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        .Where(t => t.ID == this.OrderID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();

                    orderType = (Enums.OrderType)order.Type;
                    orderStatus = (Enums.OrderStatus)order.OrderStatus;
                    this.OrderStatus = (Enums.OrderStatus)order.OrderStatus;

                    //查出 OrderControl 表、OrderControlStep 表中与该订单相关的数据
                    Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls = null;
                    Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps = null;

                    oldOrderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(t => t.OrderID == this.OrderID).ToArray();
                    if (oldOrderControls != null && oldOrderControls.Any())
                    {
                        string[] oldOrderControlIDs = oldOrderControls.Select(t => t.ID).ToArray();
                        oldOrderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => oldOrderControlIDs.Contains(t.OrderControlID)).ToArray();
                    }

                    foreach (var singleOrderItem in this.OrderItemInfos)
                    {
                        //检查 OrderItem 中是否存在，若存在，记下 OrderItem 中的 UnitPrice
                        var oldOrderItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                            .Where(t => t.ID == singleOrderItem.OriginalOrderItemID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();

                        orderItemUnitPrice = oldOrderItem.UnitPrice;
                        oldOrderItem.Quantity = singleOrderItem.OriginalQty;

                        //外单查询出型号信息，用于拼接挂起的原因，需要展示给客户确认时看
                        var orderItemInfo = new Models.OrderItem
                        {
                            ID = oldOrderItem.ID,
                            Model = oldOrderItem.Model,
                            Manufacturer = oldOrderItem.Manufacturer,
                            Quantity = oldOrderItem.Quantity,
                        };

                        IChangeQuantityDo changeQuantityDo = new ArrivalOutsideChangeQuantity(reponsitory, singleOrderItem.OriginalOrderItemID, singleOrderItem.NowQty, orderItemUnitPrice, this.OrderID, orderStatus, orderItemInfo,
                      oldOrderControls, oldOrderControlSteps);
                        if (orderType == Enums.OrderType.Outside)
                        {
                            changeQuantityDo = new ArrivalOutsideChangeQuantity(reponsitory, singleOrderItem.OriginalOrderItemID, singleOrderItem.NowQty, orderItemUnitPrice, this.OrderID, orderStatus, orderItemInfo,
                                oldOrderControls, oldOrderControlSteps);
                        }
                        else
                        {
                            this.Msg = "订单类型不是一个已知的类型";
                            return false;
                        }

                        changeQuantityDo.Do();

                    }


                    //将原“代理报关委托书”置为 400
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.MainOrderFiles>(new
                    {
                        Status = (int)Enums.Status.Delete,
                    }, item => item.MainOrderID == order.MainOrderId
                    && item.FileType == (int)Enums.FileType.AgentTrustInstrument
                    && item.Status == (int)Enums.Status.Normal);

                    reponsitory.Submit();
                }


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

                throw new Exception("ChangeQuantity 内执行错误：" + ex.Message);
            }


            return true;
        }


        /// <summary>
        /// 调用代仓储前端接口，修改型号数量
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
                //confirm.Quantity = this.NewQuantity;
                confirm.Type = ConfirmType.UpdateQuantity;

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
                    log.Summary = "订单修改型号数量推送代仓储失败:" + message.data;
                    log.Enter();
                }
            }
        }
    }
}
