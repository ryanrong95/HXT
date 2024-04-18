using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 根据到货信息，确认订单
    /// </summary>
    public class MatchConfirmOrder 
    {
        /// <summary>
        /// 没有匹配到货的订单项的ID
        /// </summary>
        public List<string> UnMatchedOrderItemIDs { get; set; }
        /// <summary>
        /// 拆分之前的订单
        /// </summary>
        public Order OriginOrder { get; set; }
        /// <summary>
        /// 页面跟单提交的 到货 订单 匹配信息
        /// </summary>
        public List<CgDeliveriesTopViewModel> SelectedItems { get; set; }

        public List<OrderItemChangeCauseOrderChange> OrderChangeItems { get; set; }
        public List<OrderItemChangeCauseOrderChange> QtyChangedItems { get; set; }
        public List<ClassifyProduct> ClassifyProducts { get; set; }
        /// <summary>
        /// OrderItem 持久化
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder OrderItemPersistenceEvent;

        /// <summary>
        /// 订单变更，新增了OrderItem 
        /// 插入OrderChangeNotices
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder OrderChangeEvent;

        /// <summary>
        /// 新增的OrderItem 自动归类
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder AutoClassifyEvent;
      
        /// <summary>
        /// 产品变更，需要插入产品变更OrderItemChangeNotices        
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder ProductChangeEvent;

        /// <summary>
        /// 产品变更，copy 原来的归类信息
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder CopyClassifyResultEvent;
        public event OrderDeliveryConfirmedEventHanlder Post2WareHouseEvent;
        public event OrderDeliveryConfirmedEventHanlder PostCurrentOrderInfoEvent;
        public event OrderDeliveryConfirmedEventHanlder SyncClassifyResultEvent;

        public MatchConfirmOrder(List<CgDeliveriesTopViewModel> selectedItems, Order originOrder, List<string> unMatchedOrderItemIDs)
        {
            this.QtyChangedItems = new List<OrderItemChangeCauseOrderChange>();
            this.OrderChangeItems = new List<OrderItemChangeCauseOrderChange>();
            this.ClassifyProducts = new List<ClassifyProduct>();
            this.SelectedItems = selectedItems;
            this.OriginOrder = originOrder;
            this.UnMatchedOrderItemIDs = unMatchedOrderItemIDs;

            this.OrderItemPersistenceEvent += ItemEnter;          
            this.OrderChangeEvent += OrderChange;
            this.AutoClassifyEvent += OrderChangeAutoClassify;
            this.ProductChangeEvent += ProductChange;
            this.CopyClassifyResultEvent += CopyClassifyResult;
            this.Post2WareHouseEvent += Post2Warehouse;
            this.PostCurrentOrderInfoEvent += PostCurrentOrderInfo;
            this.SyncClassifyResultEvent += SyncClassifyResult;
        }
        public void Confirm()
        {
            var OriginItems = OriginOrder.Items;
            List<OrderItemAssitant> ItemAssistants = new List<OrderItemAssitant>();

            //没有匹配 订单项的，需要生成新的OrderItem,自动归类，并且生成订单变更
            foreach (var orderitem in SelectedItems.Where(t => string.IsNullOrEmpty(t.OrderItemID)))
            {
                OrderItemAssitant assistant = new OrderItemAssitant();
                assistant.PersistenceType = PersistenceType.Insert;
                assistant.Name = orderitem.Name;
                assistant.Model = orderitem.Model;
                assistant.Manufacturer = orderitem.Manufacturer;
                assistant.Batch = orderitem.Batch;
                assistant.Quantity = orderitem.Quantity;
                assistant.Unit = orderitem.Unit;
                assistant.Origin = orderitem.Origin;
                assistant.UnitPrice = orderitem.UnitPrice.Value;
                assistant.TotalPrice = orderitem.TotalPrice.Value;
                assistant.ChangeType = Enums.MatchChangeType.OrderChange;
                assistant.InputID = orderitem.ID;
                ItemAssistants.Add(assistant);
            }

            //匹配订单项的，只要生成产品变更
            var selectedItemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.OrderItemID)).Select(t => t.OrderItemID).Distinct().ToList();

            foreach (var orderItemID in selectedItemIDs)
            {
                var originItem = OriginItems.Where(t => t.ID == orderItemID).FirstOrDefault();
                //这个OrderItem 在勾选的到货信息中有几条
                var orderItemCount = SelectedItems.Count(t => t.OrderItemID == orderItemID);

                if (orderItemCount == 1)
                {
                    var p = SelectedItems.Where(t => t.OrderItemID == orderItemID).FirstOrDefault();
                    //如果这个OrderItem 在到货信息中，只匹配了一条记录
                    var assistant = AssemblyItemAssistant(originItem, p);
                    assistant.PersistenceType = PersistenceType.Update;
                    assistant.ID = originItem.ID;
                    assistant.OrderID = OriginOrder.ID;
                    assistant.MatchedOrderItemID = originItem.ID;
                    if (p.Quantity != originItem.Quantity)
                    {
                        OrderItemChangeCauseOrderChange qtyChange = new OrderItemChangeCauseOrderChange();
                        qtyChange.OriginalOrderItemID = originItem.ID;
                        qtyChange.OriginalModel = originItem.Model;
                        qtyChange.OriginalQty = originItem.Quantity;
                        qtyChange.NowQty = p.Quantity;
                        qtyChange.ReasonType = OrderChangeCasuedReason.ChangeQty;
                        this.QtyChangedItems.Add(qtyChange);
                        this.OrderChangeItems.Add(qtyChange);
                    }
                    ItemAssistants.Add(assistant);
                }
                else
                {
                    //一个订单项，匹配了多条到货，这几个到货的型号必定是一模一样的
                    //这个OrderItem 在到货信息中，匹配了多条记录，就看产地信息
                    var SameIDItems = SelectedItems.Where(t => t.OrderItemID == orderItemID);
                    var ItemPlaces = SelectedItems.Where(t => t.OrderItemID == orderItemID).Select(t => t.Origin).Distinct().ToList();

                    //产地中，是否有和原来订单中产地一致的
                    if (ItemPlaces.Contains(originItem.Origin))
                    {
                        //如果有，则用到货产地 和 原订单中一致的那条，更新原来的OrderItem
                        foreach (var place in ItemPlaces)
                        {
                            var p = SameIDItems.Where(t => t.Origin == place).FirstOrDefault();
                            var assistant = AssemblyItemAssistant(originItem, p);
                            assistant.OrderID = OriginOrder.ID;
                            if (place == originItem.Origin)
                            {
                                assistant.PersistenceType = PersistenceType.Update;
                                assistant.ID = originItem.ID;
                            }
                            else
                            {
                                assistant.PersistenceType = PersistenceType.Insert;
                            }
                            assistant.Quantity = SameIDItems.Where(t => t.Origin == place).Sum(t => t.Quantity);
                            assistant.TotalPrice = p.UnitPrice.Value * assistant.Quantity;
                            assistant.MatchedOrderItemID = originItem.ID;
                            assistant.InputID = string.Join(",", SameIDItems.Where(t => t.Origin == place).Select(t => t.ID).ToArray());
                            ItemAssistants.Add(assistant);
                        }
                    }
                    else
                    {
                        //如果没有，则用到货的第一条 和 原订单Item 匹配，更新
                        for (int i = 0; i < ItemPlaces.Count(); i++)
                        {
                            var p = SameIDItems.Where(t => t.Origin == ItemPlaces[i]).FirstOrDefault();
                            var assistant = AssemblyItemAssistant(originItem, p);
                            assistant.OrderID = OriginOrder.ID;
                            if (i == 0)
                            {
                                assistant.PersistenceType = PersistenceType.Update;
                                assistant.ID = originItem.ID;
                            }
                            else
                            {
                                assistant.PersistenceType = PersistenceType.Insert;
                            }
                            assistant.Quantity = SameIDItems.Where(t => t.Origin == ItemPlaces[i]).Sum(t => t.Quantity);
                            assistant.TotalPrice = p.UnitPrice.Value * assistant.Quantity;
                            assistant.MatchedOrderItemID = originItem.ID;
                            assistant.InputID = string.Join(",", SameIDItems.Where(t => t.Origin == ItemPlaces[i]).Select(t => t.ID).ToArray());
                            ItemAssistants.Add(assistant);
                        }
                    }

                    //判断数量是否变更
                    if (SameIDItems.Sum(t => t.Quantity) != originItem.Quantity)
                    {
                        OrderItemChangeCauseOrderChange qtyChange = new OrderItemChangeCauseOrderChange();
                        qtyChange.OriginalOrderItemID = originItem.ID;
                        qtyChange.OriginalModel = originItem.Model;
                        qtyChange.OriginalQty = originItem.Quantity;
                        qtyChange.NowQty = SameIDItems.Sum(t => t.Quantity);
                        qtyChange.ReasonType = OrderChangeCasuedReason.ChangeQty;
                        this.QtyChangedItems.Add(qtyChange);
                        this.OrderChangeItems.Add(qtyChange);
                    }
                }
            }

            OnConfirmed(ItemAssistants);
        }
        private OrderItemAssitant AssemblyItemAssistant(OrderItem originItem, CgDeliveriesTopViewModel p)
        {
            OrderItemAssitant assistant = new OrderItemAssitant();
            assistant.PersistenceType = PersistenceType.Insert;
            assistant.Name = p.Name;
            assistant.Model = p.Model;
            assistant.Manufacturer = p.Manufacturer;
            assistant.Batch = p.Batch;
            assistant.Quantity = p.Quantity;
            assistant.Unit = p.Unit;
            assistant.Origin = p.Origin;
            assistant.UnitPrice = p.UnitPrice.Value;
            assistant.TotalPrice = p.TotalPrice.Value;
            assistant.ChangeType = Enums.MatchChangeType.NoChange;
            assistant.MatchedOrderItemID = p.OrderItemID;
            assistant.InputID = p.ID;

            if (p.Model != originItem.Model)
            {
                OrderItemChange change = new OrderItemChange();
                change.OrderItemChangeType = Enums.OrderItemChangeType.ProductModelChange;
                change.OldValue = originItem.Model;
                change.NewValue = p.Model;

                assistant.OrderItemChanges.Add(change);
                assistant.ChangeType = Enums.MatchChangeType.ProductChange;
            }

            if (p.Origin != originItem.Origin)
            {
                OrderItemChange change = new OrderItemChange();
                change.OrderItemChangeType = Enums.OrderItemChangeType.OriginChange;
                change.OldValue = originItem.Origin;
                change.NewValue = p.Origin;

                assistant.OrderItemChanges.Add(change);
                assistant.ChangeType = Enums.MatchChangeType.ProductChange;
            }

            if (p.Manufacturer != originItem.Manufacturer)
            {
                OrderItemChange change = new OrderItemChange();
                change.OrderItemChangeType = Enums.OrderItemChangeType.BrandChange;
                change.OldValue = originItem.Manufacturer;
                change.NewValue = p.Manufacturer;

                assistant.OrderItemChanges.Add(change);
                assistant.ChangeType = Enums.MatchChangeType.ProductChange;
            }

            //if (p.Batch != originItem.Batch && p.Batch != null && p.Batch != "")
            //{
            //    OrderItemChange change = new OrderItemChange();
            //    change.OrderItemChangeType = Enums.OrderItemChangeType.BatchChange;
            //    change.OldValue = originItem.Batch;
            //    change.NewValue = p.Batch;

            //    assistant.OrderItemChanges.Add(change);
            //    assistant.ChangeType = Enums.MatchChangeType.ProductChange;
            //}

            return assistant;
        }
        private void OnConfirmed(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderItemPersistenceEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));          
        }
        private void ItemEnter(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var matchOrder = new MatchOrderItemRelated(e.OrderItems, OriginOrder.ID);
            List<Models.OrderItemAssitant> OrderItems = matchOrder.Enter();
            OnPersistenced(OrderItems);
        }
        public void OnPersistenced(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderChangeEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.AutoClassifyEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.ProductChangeEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.CopyClassifyResultEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            DeleteModel();
            ChangeQuantity();
            this.Post2WareHouseEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.PostCurrentOrderInfoEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            UpdateOrderDeclareFlag();           
            this.SyncClassifyResultEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            SyncAutoClassifyResult();
        }

        /// <summary>
        /// 修改数量
        /// 删除型号
        /// 新增型号
        /// 重新归类引起的关税，增值税变更，不在这里插入OrderChange,是在重新归类那里插入OrderChange
        /// </summary>
        private void OrderChange(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var orderItems = e.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.OrderChange);
            foreach (var orderitem in orderItems)
            {
                OrderItemChangeCauseOrderChange qtyChange = new OrderItemChangeCauseOrderChange();
                qtyChange.OriginalOrderItemID = orderitem.ID;
                qtyChange.OriginalModel = orderitem.Model;
                qtyChange.ReasonType = OrderChangeCasuedReason.AddOrderItem;
                qtyChange.Origin = orderitem.Origin;
                this.OrderChangeItems.Add(qtyChange);
            }

            foreach (var p in this.UnMatchedOrderItemIDs)
            {
                var item = OriginOrder.Items.Where(t => t.ID == p).FirstOrDefault();
                if (item != null)
                {
                    OrderItemChangeCauseOrderChange qtyChange = new OrderItemChangeCauseOrderChange();
                    qtyChange.OriginalOrderItemID = p;
                    qtyChange.OriginalModel = item.Model;
                    qtyChange.ReasonType = OrderChangeCasuedReason.DeleteOrderItem;
                    this.OrderChangeItems.Add(qtyChange);
                }
            }

            MatchOrderChange orderChange = new MatchOrderChange(this.OrderChangeItems, this.OriginOrder);
            orderChange.Change();
        }
        //自动归类，生成订单变更
        private void OrderChangeAutoClassify(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchAutoClassify autoClassify = new MatchAutoClassify(e.OrderItems, OriginOrder);
            List<ClassifyProduct> classifyProducts = autoClassify.Classify();
            this.ClassifyProducts = classifyProducts;
        }
       
       
        private void ProductChange(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchProductChange productChange = new MatchProductChange(e.OrderItems, OriginOrder);
            productChange.Change();
        }
        private void CopyClassifyResult(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchCopyClassifyResult copyClassifyResult = new MatchCopyClassifyResult(e.OrderItems, OriginOrder, OriginOrder.ID);
            copyClassifyResult.Copy();
        }
        private void UpdateOrderDeclareFlag()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    DeclareFlag = (int)Enums.DeclareFlagEnums.Able
                }, item => item.ID == OriginOrder.ID);
                reponsitory.Submit();
            }
        }
        private void Post2Warehouse(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchPost2Warehouse post2Warehouse = new MatchPost2Warehouse(e.OrderItems, OriginOrder);
            post2Warehouse.Post();

        }
        private void PostCurrentOrderInfo(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            List<OrderItemAssitant> orderItemAssitants = e.OrderItems;
            List<string> splitOrderItems = new List<string>();
            foreach (var item in orderItemAssitants.Where(t => t.PersistenceType == PersistenceType.Insert).ToList())
            {
                if (item.MatchedOrderItemID != null)
                {
                    splitOrderItems.Add(item.MatchedOrderItemID);
                }                
            }
            MatchPost2AgentWarehouse post2AgentWarehouse = new MatchPost2AgentWarehouse(OriginOrder.MainOrderID, OriginOrder, splitOrderItems);
            post2AgentWarehouse.Post();
        }
        private void DeleteModel()
        {
            if (this.UnMatchedOrderItemIDs.Count() > 0)
            {
                ArrivalDeleteModelHandler deleteModelHandler = new ArrivalDeleteModelHandler(this.UnMatchedOrderItemIDs, OriginOrder.ID);
                bool result = deleteModelHandler.Execute();
                if (result)
                {
                    if (deleteModelHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Classified)
                    {
                        try
                        {
                            var bill = OriginOrder.MainOrderFiles.Where(file => file.FileType == FileType.OrderBill && file.Status == Status.Normal).SingleOrDefault();

                            bill?.Abandon();
                            OriginOrder.GenerateBill();
                        }
                        catch (Exception e)
                        {

                            throw new Exception("重新生成对账单内执行错误：" + e.Message);
                        }
                    }
                    else if (deleteModelHandler.OrderStatus == Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                    {
                        deleteModelHandler.UpdateOrder(Needs.Underly.FkoFactory<Admin>.Create(OriginOrder.Client.Merchandiser.ID), deleteModelHandler.OrderType);
                    }
                }
                else
                {

                }
            }

        }
        private void ChangeQuantity()
        {
            if (this.QtyChangedItems.Count() > 0)
            {
                ArrivalChangeQuantityHandler changeQuantityHandler = new ArrivalChangeQuantityHandler(this.QtyChangedItems, OriginOrder.ID);
                bool result = changeQuantityHandler.Execute();
                if (result)
                {
                    try
                    {
                        if (changeQuantityHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                        {
                            var bill = OriginOrder.MainOrderFiles.Where(file => file.FileType == FileType.OrderBill && file.Status == Status.Normal).SingleOrDefault();

                            bill?.Abandon();
                            OriginOrder.GenerateBill();
                        }
                    }
                    catch (Exception e)
                    {

                        throw new Exception("重新生成对账单内执行错误：" + e.Message);
                    }
                }
            }

        }

        private void SyncClassifyResult(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var orderItems = e.OrderItems.Where(t => t.ChangeType != Enums.MatchChangeType.OrderChange && t.PersistenceType == PersistenceType.Insert).ToList();
            try
            {
                if (orderItems.Count() > 0)
                {
                    MatchSyncClassify matchSyncClassify = new MatchSyncClassify(orderItems);
                    matchSyncClassify.SyncResult();
                }               
            }
            catch(Exception ex)
            {

            }                    
        }

        private void SyncAutoClassifyResult()
        {
            MatchSyncAutoClassify matchSyncAutoClassify = new MatchSyncAutoClassify(this.ClassifyProducts);
            matchSyncAutoClassify.SyncResult();
        }
    }
}
