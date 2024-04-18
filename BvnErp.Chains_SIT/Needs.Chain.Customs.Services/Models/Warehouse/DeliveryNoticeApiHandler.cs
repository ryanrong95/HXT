using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 到货通知接口 Handler
    /// </summary>
    public class DeliveryNoticeApiHandler
    {
        public bool IsOK { get; set; } = true;

        public bool IsReturnOK { get; set; } = false; //是否返回成功

        public string Msg { get; set; } = string.Empty;

        public List<Views.DeliveriesTopModelOriginName> NeedOrderItemsInView { get; set; }

        List<Models.OrderItem> NeedOrderItemsInLocal { get; set; }

        public string[] OldOrderItemIDs { get; set; }

        /// <summary>
        /// 拆分出的 OrderItemIDs
        /// </summary>
        public List<OrderItemPair> NewOrderItemPairs { get; set; } = new List<OrderItemPair>();

        public List<Views.DeliveriesTopModelOriginName> OrderItemInView返回代仓储 { get; set; }

        public List<Models.OrderItem> OrderItemInLocal返回代仓储 { get; set; }

        /// <param name="orderID"></param>
        public DeliveryNoticeApiHandler(string orderID, string batchID)
        {
            try
            {
                //从 DeliveriesTopView 视图中查出大订单下所有的到货型号信息
                var orderItemsInView = new Views.DeliveriesTopView().GetDataByOrderID(orderID).ToList();

                if (orderItemsInView != null && orderItemsInView.Any())
                {
                    new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        BatchID = batchID,
                        OrderID = orderID,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "DeliveriesTopView视图 || " + orderItemsInView.Json(),
                    }.Enter();
                }

                //从本地查出大订单下所有的型号信息
                var orderItemsInLocal = new Views.OrderItemsInMainOrderView().GetOrderItemsByMainOrderID(orderID).ToList();
                if (orderItemsInLocal != null && orderItemsInLocal.Any())
                {
                    new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        BatchID = batchID,
                        OrderID = orderID,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "OrderItems表 || " + orderItemsInLocal.Json(),
                    }.Enter();

                    for (int i = 0; i < orderItemsInLocal.Count; i++)
                    {
                        if (string.IsNullOrEmpty(orderItemsInLocal[i].Batch))
                        {
                            orderItemsInLocal[i].Batch = string.Empty;
                        }
                        if (string.IsNullOrEmpty(orderItemsInLocal[i].Origin))
                        {
                            orderItemsInLocal[i].Origin = string.Empty;
                        }
                    }
                }

                //求出两个集合中的 OrderItemID 交集并去重
                string[] orderItemIDs = (from orderItemView in orderItemsInView
                                         join orderItemLocal in orderItemsInLocal on orderItemView.IptItemID equals orderItemLocal.ID
                                         select orderItemView.IptItemID).ToArray();

                orderItemIDs = orderItemIDs.Distinct().ToArray();

                //如果两个集合并没有 OrderItemID 交集，返回执行成功
                if (orderItemIDs == null || !orderItemIDs.Any())
                {
                    this.IsReturnOK = true;
                    this.IsOK = true;
                    this.Msg = "大订单 OrderID = " + orderID + " 视图和本地数据中，OrderItemID 无交集";
                    return;
                }

                //检查每个 OrderItemID 对应的 View 中的数据，“同一个 OrderItemID 如果出现多条数据，只允许产地不一样”
                List<string> 有效的ItemIDs = new List<string>();
                foreach (var orderItemID in orderItemIDs)
                {
                    var theOrderItems = orderItemsInView.Where(t => t.IptItemID == orderItemID).ToArray();

                    if (theOrderItems.Length <= 1)
                    {
                        有效的ItemIDs.Add(orderItemID);
                        continue;
                    }

                    if (!OneOrderItemFiledShouldInLine(theOrderItems))
                    {
                        //this.IsOK = false;
                        //this.Msg = "iptItemID = " + orderItemID + " 的数据中，存在型号或品牌不同的情况，请检查";
                        //return;

                        new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            BatchID = batchID,
                            OrderID = orderID,
                            Status = Needs.Ccs.Services.Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = "排除 OrderItemID = " + orderItemID,
                        }.Enter();
                    }
                    else
                    {
                        有效的ItemIDs.Add(orderItemID);
                    }
                }
                orderItemIDs = 有效的ItemIDs.ToArray();
                if (orderItemIDs == null || !orderItemIDs.Any())
                {
                    this.IsReturnOK = true;
                    this.IsOK = true;
                    this.Msg = "大订单 OrderID = " + orderID + " 视图和本地数据中，OrderItemID 无有效交集";
                    return;
                }

                this.OrderItemInView返回代仓储 = orderItemsInView.Where(t => orderItemIDs.Contains(t.IptItemID)).ToList();
                this.OrderItemInLocal返回代仓储 = orderItemsInLocal.Where(t => orderItemIDs.Contains(t.ID)).ToList();

                //只取 Local 和 View 中 OrderItemID 交集的数据
                //并且将 同一个 OrderItemID 的多个数据中，批次号使用第一个
                var needOrderItemsInView = SetTheSameIptDateCodeByOrderItemID(orderItemsInView.ToArray(), orderItemIDs);

                //将信息相同的数据，数量累加，其他信息不变
                needOrderItemsInView = (from orderItem in needOrderItemsInView
                                        group orderItem by new
                                        {
                                            orderItem.IptOrderID,
                                            orderItem.IptTinyOrderID,
                                            orderItem.IptItemID,
                                            orderItem.PtvPartNumber,
                                            orderItem.PtvManufacturer,
                                            orderItem.IptOrigin,
                                            orderItem.OriginCode,
                                            orderItem.IptDateCode,
                                        } into g
                                        select new Views.DeliveriesTopModelOriginName()
                                        {
                                            IptOrderID = g.Key.IptOrderID,
                                            IptTinyOrderID = g.Key.IptTinyOrderID,
                                            IptItemID = g.Key.IptItemID,
                                            PtvPartNumber = g.Key.PtvPartNumber,
                                            PtvManufacturer = g.Key.PtvManufacturer,
                                            IptOrigin = g.Key.IptOrigin,
                                            OriginCode = g.Key.OriginCode,
                                            IptDateCode = g.Key.IptDateCode,
                                            StoQuantity = g.Sum(t => t.StoQuantity),
                                            SorterAdmin = g.FirstOrDefault().SorterAdmin,
                                            InputIDs = g.Select(t => t.StoInputID).ToList(),
                                        }).ToList();


                var needOrderItemsInLocal = orderItemsInLocal.Where(t => orderItemIDs.Contains(t.ID)).ToList();


                this.OldOrderItemIDs = orderItemIDs;
                this.NeedOrderItemsInView = needOrderItemsInView;
                this.NeedOrderItemsInLocal = needOrderItemsInLocal;
            }
            catch (Exception ex)
            {
                ex.CcsLog("到货通知接口Handler(DeliveryNoticeApiHandler)初始化发生异常, 大订单ID = " + orderID);
                this.IsOK = false;
                this.Msg = "大订单ID = " + orderID + " 到货通知获取接口，初始化发生异常";
                return;
            }
        }

        /// <summary>
        /// 检查同一个 OrderItemInView 的数据，型号和品牌应该相同
        /// “同一个 OrderItemID 如果出现多条数据，只允许产地不一样”(即型号、品牌应该相同，批次号可以不一样)
        /// </summary>
        private bool OneOrderItemFiledShouldInLine(Views.DeliveriesTopModelOriginName[] oneOrderItemData)
        {
            if (oneOrderItemData == null || !oneOrderItemData.Any())
            {
                return true;
            }

            string[] ptvPartNumbers = oneOrderItemData.Select(t => t.PtvPartNumber).Distinct().ToArray();
            if (ptvPartNumbers.Length > 1)
            {
                return false;
            }

            string[] ptvManufacturer = oneOrderItemData.Select(t => t.PtvManufacturer).Distinct().ToArray();
            if (ptvManufacturer.Length > 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将相同 OrderItemID 的多个数据中，IptDateCode 设置为同一个 IptDateCode
        /// </summary>
        private List<Views.DeliveriesTopModelOriginName> SetTheSameIptDateCodeByOrderItemID(
            Views.DeliveriesTopModelOriginName[] allDeliveriesTopModels, string[] orderItemIDs)
        {
            List<Needs.Ccs.Services.Views.DeliveriesTopModelOriginName> resultDeliveriesTopModel = new List<Needs.Ccs.Services.Views.DeliveriesTopModelOriginName>();

            foreach (var orderItemID in orderItemIDs)
            {
                var theOrderItems = allDeliveriesTopModels.Where(t => t.IptItemID == orderItemID).ToArray();

                if (theOrderItems == null)
                {
                    continue;
                }

                if (theOrderItems.Length == 1)
                {
                    resultDeliveriesTopModel.Add(theOrderItems[0]);
                    continue;
                }

                string theIptDateCode = theOrderItems.FirstOrDefault().IptDateCode;
                for (int i = 0; i < theOrderItems.Length; i++)
                {
                    theOrderItems[i].IptDateCode = theIptDateCode;
                }

                resultDeliveriesTopModel.AddRange(theOrderItems);
            }

            return resultDeliveriesTopModel;
        }


        public void GenerateOrderItemChangeAndSplit()
        {
            try
            {
                for (int i = 0; i < this.OldOrderItemIDs.Length; i++)
                {

                    var theOrderItemsInView = this.NeedOrderItemsInView.Where(t => t.IptItemID == this.OldOrderItemIDs[i]).ToList();
                    var theOrderItemInLocal = this.NeedOrderItemsInLocal.Where(t => t.ID == this.OldOrderItemIDs[i]).FirstOrDefault();

                    if (theOrderItemsInView == null || theOrderItemInLocal == null)
                    {
                        continue;
                    }

                    if (theOrderItemsInView.Count == 1)
                    {
                        //1.同一个 OrderItemID 数据数量相等的
                        //比对 型号、品牌、产地、批次号 这4个信息，针对每一个，如果有变化，就生成对应的 变更信息。

                        ChangeOrderItemInfo(theOrderItemsInView[0], theOrderItemInLocal);

                    }
                    else
                    {
                        //2.同一个 OrderItemID 数据数量不相等的(>= 2)
                        //找出与原先产地不同的，应该肯定会有与原先产地不同的

                        //这些与原先原产地不同的数据，每一个都拆分出一个新的 OrderItemID。

                        //然后，这些 OrderItem 中，根据产地对应，应该是一对一的(Local 对 View)，比对每一对数据的 型号、品牌、批次号，如果有变化，就生成对应的 变更信息。


                        List<OrderItemPair> thePartNewOrderItems = new List<OrderItemPair>();

                        SplitModel(theOrderItemsInView.ToArray(), theOrderItemInLocal.ID, out thePartNewOrderItems);

                        this.NewOrderItemPairs.AddRange(thePartNewOrderItems);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("生成型号变更通知或拆分型号执行方法中发生异常(GenerateOrderItemChangeAndSplit)");
                this.IsOK = false;
                this.Msg = "生成型号变更通知或拆分型号执行方法中发生异常(GenerateOrderItemChangeAndSplit)";
                return;
            }
        }

        /// <summary>
        /// 2.同一个 OrderItemID 数据数量不相等的(>= 2)
        /// 找出与原先产地不同的，应该肯定会有与原先产地不同的
        /// 这些与原先原产地不同的数据，每一个都拆分出一个新的 OrderItemID。
        /// 然后，这些 OrderItem 中，根据产地对应，应该是一对一的(Local 对 View)，比对每一对数据的 型号、品牌、批次号，如果有变化，就生成对应的 变更信息。
        /// </summary>
        /// <param name="orderItemsInView"></param>
        /// <param name="orderItemLocal"></param>
        private void SplitModel(Views.DeliveriesTopModelOriginName[] orderItemsInView, string orderItemInLocalID, out List<OrderItemPair> outNewOrderItemIDs)
        {
            List<OrderItemPair> listNewOrderItemID = new List<OrderItemPair>();


            for (int i = 0; i < orderItemsInView.Length; i++)
            {
                var orderItemInLocal = new Views.OrderItemsView()[orderItemInLocalID];

                if (orderItemsInView[i].OriginCode == orderItemInLocal.Origin)
                {
                    ChangeOrderItemInfo(orderItemsInView[0], orderItemInLocal);
                }
                else if (orderItemsInView[i].OriginCode != orderItemInLocal.Origin && orderItemInLocal.Quantity >= orderItemsInView[i].StoQuantity)
                {
                    string origin = orderItemsInView[i].OriginCode;
                    string manufacturer = orderItemInLocal.Manufacturer; //品牌先不变
                    decimal quantity = orderItemsInView[i].StoQuantity;
                    var admin = orderItemsInView[i].SorterAdmin;

                    string newOrderItemID = orderItemInLocal.SplitModel2(origin, quantity, manufacturer, admin);

                    listNewOrderItemID.Add(new OrderItemPair()
                    {
                        NewOrderItemID = newOrderItemID,
                        OldOrderItemID = orderItemInLocalID,
                        TargetOrgin = origin,
                        TargetQuantity = quantity,
                        Inputs = orderItemsInView[i].InputIDs,
                        TinyOrderID = orderItemsInView[i].IptTinyOrderID,
                    });

                    var newOrderItemInLocal = new Views.OrderItemsView()[newOrderItemID];

                    ChangeOrderItemInfo(orderItemsInView[i], newOrderItemInLocal);
                }


            }

            outNewOrderItemIDs = listNewOrderItemID;
        }

        /// <summary>
        /// 修改型号信息
        /// </summary>
        /// <param name="orderItemInView"></param>
        /// <param name="orderItemInLocal"></param>
        private void ChangeOrderItemInfo(Views.DeliveriesTopModelOriginName orderItemInView, Models.OrderItem orderItemInLocal)
        {
            if (orderItemInView.PtvPartNumber != orderItemInLocal.Model)
            {
                orderItemInLocal.SorterAdmin = orderItemInView.SorterAdmin;
                orderItemInLocal.ChangeProductModel(orderItemInView.PtvPartNumber);
            }

            if (orderItemInView.PtvManufacturer != orderItemInLocal.Manufacturer)
            {
                orderItemInLocal.SorterAdmin = orderItemInView.SorterAdmin;
                orderItemInLocal.ChangeManufacturer(orderItemInView.PtvManufacturer);
            }

            if (orderItemInView.OriginCode != orderItemInLocal.Origin)
            {
                orderItemInLocal.SorterAdmin = orderItemInView.SorterAdmin;
                orderItemInLocal.ChangeOrigin(orderItemInView.OriginCode);
            }

            if (orderItemInView.IptDateCode != orderItemInLocal.Batch)
            {
                orderItemInLocal.SorterAdmin = orderItemInView.SorterAdmin;
                orderItemInLocal.ChangeBatch(orderItemInView.IptDateCode);
            }
        }

    }

    public class OrderItemPair
    {
        public string NewOrderItemID { get; set; }

        public string OldOrderItemID { get; set; }

        public string TargetOrgin { get; set; }

        public decimal TargetQuantity { get; set; }

        public List<string> Inputs { get; set; }

        public string TinyOrderID { get; set; }
    }


}
