using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ChangeQuantityHandler
    {
        private string OrderItemID { get; set; } = string.Empty;

        private string OrderID { get; set; } = string.Empty;

        private int NewQuantity { get; set; }

        public string Msg { get; set; } = string.Empty;

        public Enums.OrderStatus OrderStatus { get; set; }
        public int OldQuantity { get; set; }
        public ChangeQuantityHandler(string orderItemID, string orderID, int newQuantity,int oldQuantity=0)
        {
            this.OrderItemID = orderItemID;
            this.OrderID = orderID;
            this.NewQuantity = newQuantity;
            this.OldQuantity = oldQuantity;
        }

        public bool Execute()
        {
            try
            {
                decimal orderItemUnitPrice = 0;
                Enums.OrderType orderType;
                Enums.OrderStatus orderStatus;  //根据订单状态判断是否挂起订单（>= QuoteConfirmed  4  可挂起订单）

                bool isHasHkSortings = false; //用这个判断这个型号是否装箱
                string sortingIDForInside = string.Empty;


                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    //检查 Order 中是否存在，若存在，记下 订单类型
                    var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        .Where(t => t.ID == this.OrderID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                    if (order == null)
                    {
                        this.Msg = "不存在 OrderID = " + this.OrderID + " 的订单";
                        return false;
                    }
                    orderType = (Enums.OrderType)order.Type;
                    orderStatus = (Enums.OrderStatus)order.OrderStatus;
                    this.OrderStatus = (Enums.OrderStatus)order.OrderStatus;
                    if (orderStatus == Enums.OrderStatus.Draft)
                    {
                        this.Msg = "订单 " + this.OrderID + " 为草稿状态，请到订单草稿中操作";
                        return false;
                    }
                    //else if (orderStatus == Enums.OrderStatus.Confirmed)
                    //{
                    //    this.Msg = "订单 " + this.OrderID + " 为待归类状态，暂不支持修改型号数量";
                    //    return false;
                    //}


                    //检查 OrderItem 中是否存在，若存在，记下 OrderItem 中的 UnitPrice
                    var oldOrderItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                        .Where(t => t.ID == this.OrderItemID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                    if (oldOrderItem == null)
                    {
                        this.Msg = "不存在 OrderItemID = " + this.OrderItemID + "的型号";
                        return false;
                    }
                    orderItemUnitPrice = oldOrderItem.UnitPrice;

                    if (this.OldQuantity != 0)
                    {
                        oldOrderItem.Quantity = OldQuantity;
                    }

                    //外单查询出型号信息，用于拼接挂起的原因，需要展示给客户确认时看
                    var orderItemInfo = new Models.OrderItem
                    {
                        ID = oldOrderItem.ID,
                        Model = oldOrderItem.Model,
                        Manufacturer = oldOrderItem.Manufacturer,
                        Quantity = oldOrderItem.Quantity,
                    };




                    //查出所有香港库房的 200 的 sorting， 外单如果有的话就表示已经装箱；内单取第一个，记下 SortingID 需要修改 Sorting 中的数量
                    var sortings = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>()
                        .Where(t => t.OrderItemID == this.OrderItemID
                                 && t.WarehouseType == (int)Enums.WarehouseType.HongKong
                                 && t.Status == (int)Enums.Status.Normal).ToList();
                    if (sortings != null && sortings.Any())
                    {
                        isHasHkSortings = true;
                        sortingIDForInside = sortings.FirstOrDefault()?.ID;
                    }


                    //查出 OrderControl 表、OrderControlStep 表中与该订单相关的数据
                    Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls = null;
                    Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps = null;

                    oldOrderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(t => t.OrderID == this.OrderID).ToArray();
                    if (oldOrderControls != null && oldOrderControls.Any())
                    {
                        string[] oldOrderControlIDs = oldOrderControls.Select(t => t.ID).ToArray();
                        oldOrderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => oldOrderControlIDs.Contains(t.OrderControlID)).ToArray();
                    }

                    IChangeQuantityDo changeQuantityDo = new OutsideChangeQuantity(reponsitory, this.OrderItemID, this.NewQuantity, orderItemUnitPrice, this.OrderID, orderStatus, orderItemInfo,
                        oldOrderControls, oldOrderControlSteps);
                    if (orderType == Enums.OrderType.Outside)
                    {
                        Enums.EntryNoticeStatus hkEntryNoticeStatus = Enums.EntryNoticeStatus.UnBoxed;

                        //香港入库通知若存在，则记下 香港入库通知的状态，在外单中需要判断是否封箱
                        var hkEntryNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                            .Where(t => t.OrderID == this.OrderID
                                     && t.WarehouseType == (int)Enums.WarehouseType.HongKong
                                     && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                        if (hkEntryNotice != null)
                        {
                            hkEntryNoticeStatus = (Enums.EntryNoticeStatus)hkEntryNotice.EntryNoticeStatus;
                        }

                        //外单已封箱的话，不能修改数量
                        if (hkEntryNoticeStatus == Enums.EntryNoticeStatus.Sealed)
                        {
                            this.Msg = "该订单已封箱，不能修改数量";
                            return false;
                        }

                        //通过是否有香港的 Sorting 判断这个型号是否已装箱
                        if (isHasHkSortings)
                        {
                            this.Msg = "该型号已装箱，不能修改数量";
                            return false;
                        }

                        changeQuantityDo = new OutsideChangeQuantity(reponsitory, this.OrderItemID, this.NewQuantity, orderItemUnitPrice, this.OrderID, orderStatus, orderItemInfo,
                            oldOrderControls, oldOrderControlSteps);
                    }
                    else if (orderType == Enums.OrderType.Icgoo || orderType == Enums.OrderType.Inside)
                    {
                        var decHead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                            .Where(t => t.OrderID == this.OrderID).FirstOrDefault();
                        if (decHead != null)
                        {
                            //如果报关完成，则不让修改数量
                            if (decHead.IsSuccess)
                            {
                                this.Msg = "该订单已报关完成，不能修改数量";
                                return false;
                            }
                        }

                        string decListIDForInside = string.Empty;
                        decimal declistDeclPriceForInside = 0;

                        //查出相关 OrderItemID 的 DecList，内单需要修改 DecList 中的数量和总价
                        var decList = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                            .Where(t => t.OrderItemID == this.OrderItemID).FirstOrDefault();
                        if (decList != null)
                        {
                            decListIDForInside = decList.ID;
                            declistDeclPriceForInside = decList.DeclPrice;
                        }

                        changeQuantityDo = new InsideChangeQuantity(reponsitory, this.OrderItemID, this.NewQuantity, orderItemUnitPrice, sortingIDForInside,
                            decListIDForInside, declistDeclPriceForInside);
                        //oldOrderControls, oldOrderControlSteps);
                    }
                    else
                    {
                        this.Msg = "订单类型不是一个已知的类型";
                        return false;
                    }

                    changeQuantityDo.Do();

                    //将原“代理报关委托书”置为 400
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderFiles>(new
                    {
                        Status = (int)Enums.Status.Delete,
                    }, item => item.OrderID == this.OrderID
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
                confirm.OrderItemID = this.OrderItemID;
                confirm.Quantity = this.NewQuantity;
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

    public interface IChangeQuantityDo
    {
        void Do();
    }

}
