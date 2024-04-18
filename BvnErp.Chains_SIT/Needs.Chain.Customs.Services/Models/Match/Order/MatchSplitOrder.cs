using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchSplitOrder
    {
        /// <summary>
        /// 拆分之前的订单
        /// </summary>
        public Order OriginOrder { get; set; }
        /// <summary>
        /// 新拆出来的订单
        /// </summary>
        public Order CurrentOrder { get; set; }
        /// <summary>
        /// 页面跟单提交的 到货 订单 匹配信息
        /// </summary>
        public List<CgDeliveriesTopViewModel> SelectedItems { get; set; }
        public List<OrderItemChangeCauseOrderChange> OrderChangeItems { get; set; }
        public List<OrderItemChangeCauseOrderChange> QtyChangedItems { get; set; }
        public List<ClassifyProduct> ClassifyProducts { get; set; }

        /// <summary>
        /// 需要拆分的付汇申请项ID
        /// </summary>
        public string PayExchangeSplit { get; set; }

        public MatchSplitOrder(List<CgDeliveriesTopViewModel> selectedItems, Order originOrder, Order currentOrder, string PayExchangeSplit)
        {
            this.QtyChangedItems = new List<OrderItemChangeCauseOrderChange>();
            this.OrderChangeItems = new List<OrderItemChangeCauseOrderChange>();
            this.ClassifyProducts = new List<ClassifyProduct>();
            this.SelectedItems = selectedItems;
            this.OriginOrder = originOrder;
            this.CurrentOrder = currentOrder;
            this.PayExchangeSplit = PayExchangeSplit;

            this.OrderItemPersistenceEvent += ItemEnter;

            this.OrderChangeEvent += OrderChange;
            this.AutoClassifyEvent += OrderChangeAutoClassify;

            this.ProductChangeEvent += ProductChange;
            this.CopyClassifyResultEvent += CopyClassifyResult;

            this.Post2WareHouseEvent += Post2Warehouse;
            this.PostCurrentOrderInfoEvent += PostCurrentOrderInfo;
            this.Post2ClientEvent += Post2ClientConfirm;
            this.SyncClassifyResultEvent += SyncClassifyResult;
        }

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
        public event OrderDeliveryConfirmedEventHanlder Post2ClientEvent;
        public event OrderDeliveryConfirmedEventHanlder SyncClassifyResultEvent;

        /// <summary>
        /// 组装持久化信息
        /// </summary>
        public void Split()
        {
            var OriginItems = OriginOrder.Items;
            List<OrderItemAssitant> ItemAssistants = new List<OrderItemAssitant>();
            //没有匹配 订单项的，需要生成新的OrderItem,自动归类，并且生成订单变更
            //生成新的OrderItem有两种情况，一种是下面这种情况，没有匹配订单项，一种是产地变更，生成了新的OrderItem
            //没有匹配订单项，需要自动归类，并且生成订单变更，不需要生成产品变更
            //产地变更的，不需要自动归类，不需要生成订单变更，只要生成产品变更就行
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
                decimal selectedItemCount = SelectedItems.Where(t => t.OrderItemID == orderItemID).Sum(t => t.Quantity);

                if (selectedItemCount >= originItem.Quantity)
                {
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

                            if (place == originItem.Origin)
                            {
                                assistant.PersistenceType = PersistenceType.Update;
                                assistant.ID = originItem.ID;
                                assistant.OrderID = CurrentOrder.ID;
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

                            if (i == 0)
                            {
                                assistant.PersistenceType = PersistenceType.Update;
                                assistant.ID = originItem.ID;
                                assistant.OrderID = CurrentOrder.ID;
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
                        this.OrderChangeItems.Add(qtyChange);
                        this.QtyChangedItems.Add(qtyChange);
                    }

                }
                else if (selectedItemCount < originItem.Quantity)
                {
                    //如果到货的数量小于原始订单的数量，
                    //1、保留原OrderItem,只修改原OrderItem的数量(原OrderItem数量减去到货数量)，不管型号，原产地，品牌，批次等是否一致，都不去修改
                    //2、生成新的OrderItem，生成多少个OrderItem视原产地情况看，如果有三个原产地，则生成三个OrderItem，并且归类信息全都Copy原来的归类信息，并生成相应的产品变更记录
                    OrderItemAssitant assistant = new OrderItemAssitant();
                    assistant.PersistenceType = PersistenceType.Update;
                    assistant.ID = originItem.ID;
                    assistant.Quantity = originItem.Quantity - selectedItemCount;
                    assistant.TotalPrice = Math.Round(assistant.Quantity * originItem.UnitPrice, 2, MidpointRounding.AwayFromZero);
                    assistant.ChangeType = Enums.MatchChangeType.NoChange;
                    ItemAssistants.Add(assistant);

                    var SameIDItems = SelectedItems.Where(t => t.OrderItemID == orderItemID);
                    var ItemPlaces = SelectedItems.Where(t => t.OrderItemID == orderItemID).Select(t => t.Origin).Distinct().ToList();
                    foreach (var place in ItemPlaces)
                    {
                        var p = SameIDItems.Where(t => t.Origin == place).FirstOrDefault();
                        var passistant = AssemblyItemAssistant(originItem, p);
                        passistant.PersistenceType = PersistenceType.Insert;
                        passistant.Quantity = SameIDItems.Where(t => t.Origin == place).Sum(t => t.Quantity);
                        passistant.TotalPrice = p.UnitPrice.Value * passistant.Quantity;
                        passistant.MatchedOrderItemID = originItem.ID;
                        passistant.InputID = string.Join(",", SameIDItems.Where(t => t.Origin == place).Select(t => t.ID).ToArray());
                        ItemAssistants.Add(passistant);
                    }
                }
            }

            OnSplited(ItemAssistants);
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

            //if (p.Batch != originItem.Batch)
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

        public void OnSplited(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderItemPersistenceEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
        }

        private void ItemEnter(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                //新增一条Order记录   
                CurrentOrder.DeclareFlag = Enums.DeclareFlagEnums.Able;
                reponsitory.Insert(CurrentOrder.ToLinq());
                reponsitory.Insert(CurrentOrder.OrderConsignee.ToLinq());
                reponsitory.Insert(CurrentOrder.OrderConsignor.ToLinq());
                foreach (var payExchangeSupplier in CurrentOrder.PayExchangeSuppliers)
                {
                    reponsitory.Insert(payExchangeSupplier.ToLinq());
                }
                reponsitory.Submit();
            }

            string currentOrderID = CurrentOrder.ID;
            var matchOrder = new MatchOrderItemRelated(e.OrderItems, CurrentOrder.ID);
            List<Models.OrderItemAssitant> OrderItems = matchOrder.Enter();

            //改原订单的报关价格
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                var declarePrice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == OriginOrder.ID).Sum(t => t.TotalPrice);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { DeclarePrice = declarePrice }, item => item.ID == OriginOrder.ID);
                reponsitory.Submit();
            }

            OnPersistenced(OrderItems);
            AdjustOrderVoyages(currentOrderID, OriginOrder.ID);
            AdjustOrderFee(currentOrderID);
            AdjustOrderChangeNotices(currentOrderID, e.OrderItems);
            AdjustPayExchange(PayExchangeSplit);
        }

        public void OnPersistenced(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderChangeEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.AutoClassifyEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.ProductChangeEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.CopyClassifyResultEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.Post2WareHouseEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.PostCurrentOrderInfoEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            ChangeQuantity();
            this.SyncClassifyResultEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            SyncAutoClassifyResult();
            this.Post2ClientEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
        }

        /// <summary>
        /// 修改数量
        /// 删除型号 这里不会有删除型号
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

            MatchOrderChange orderChange = new MatchOrderChange(this.OrderChangeItems, this.CurrentOrder);
            orderChange.Change();
        }

        //自动归类，生成订单变更
        private void OrderChangeAutoClassify(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchAutoClassify autoClassify = new MatchAutoClassify(e.OrderItems, CurrentOrder);
            List<ClassifyProduct> classifyProducts = autoClassify.Classify();
            this.ClassifyProducts = classifyProducts;
        }

        private void ProductChange(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchProductChange productChange = new MatchProductChange(e.OrderItems, CurrentOrder);
            productChange.Change();
        }

        private void CopyClassifyResult(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchCopyClassifyResult copyClassifyResult = new MatchCopyClassifyResult(e.OrderItems, OriginOrder, CurrentOrder.ID);
            copyClassifyResult.Copy();
        }

        private void Post2Warehouse(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            MatchPost2Warehouse post2Warehouse = new MatchPost2Warehouse(e.OrderItems.Where(t => !string.IsNullOrEmpty(t.InputID)).ToList(), CurrentOrder);
            post2Warehouse.Post();
        }
        private void ChangeQuantity()
        {
            if (this.QtyChangedItems.Count() > 0)
            {
                ArrivalChangeQuantityHandler changeQuantityHandler = new ArrivalChangeQuantityHandler(this.QtyChangedItems, CurrentOrder.ID);
                bool result = changeQuantityHandler.Execute();
                if (result)
                {
                    try
                    {
                        if (changeQuantityHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                        {
                            var bill = CurrentOrder.MainOrderFiles.Where(file => file.FileType == FileType.OrderBill && file.Status == Status.Normal).SingleOrDefault();

                            bill?.Abandon();
                            CurrentOrder.GenerateBill();
                        }
                    }
                    catch (Exception e)
                    {

                        throw new Exception("重新生成对账单内执行错误：" + e.Message);
                    }
                }
            }

        }

        /// <summary>
        /// 只是因为分批到货，先报关一部分，其他都没有改变，没有需要重新归类的产品，没有数量变更，没有订单变更，直接发送客户确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Post2ClientConfirm(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var orderItems = e.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.ProductChange);
            if (orderItems.Count() == 0 && this.OrderChangeItems.Count == 0)
            {
                Post2Client();
            }
        }

        private void Post2Client()
        {
            MatchPost2ClientDirectConfirm directConfirm = new MatchPost2ClientDirectConfirm(CurrentOrder);
            directConfirm.Post();
        }


        /// <summary>
        /// 拆分报关给代仓储的时候，要给所有OrderItem信息，不能只给选定的OrderItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            MatchPost2AgentWarehouse post2AgentWarehouse = new MatchPost2AgentWarehouse(OriginOrder.MainOrderID, CurrentOrder, splitOrderItems);
            post2AgentWarehouse.Post();
        }

        private void SyncClassifyResult(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var orderItems = e.OrderItems.Where(t => t.ChangeType != Enums.MatchChangeType.OrderChange && t.PersistenceType == PersistenceType.Insert).ToList();
            try
            {
                if (orderItems.Count > 0)
                {
                    MatchSyncClassify matchSyncClassify = new MatchSyncClassify(orderItems);
                    matchSyncClassify.SyncResult();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SyncAutoClassifyResult()
        {
            MatchSyncAutoClassify matchSyncAutoClassify = new MatchSyncAutoClassify(this.ClassifyProducts);
            matchSyncAutoClassify.SyncResult();
        }

        private void AdjustOrderVoyages(string CurrentOrderID, string OriginOrderID)
        {
            Order CurrentOrder = new Needs.Ccs.Services.Views.Orders2View().Where(t => t.ID == CurrentOrderID).FirstOrDefault();
            if (CurrentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.HighValue).Count() > 0)
            {
                UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.HighValue, Status.Normal);
            }

            if (CurrentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Inspection).Count() > 0)
            {
                UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.Inspection, Status.Normal);
            }

            if (CurrentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Quarantine).Count() > 0)
            {
                UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.Quarantine, Status.Normal);
            }

            if (CurrentOrder.Items.Where(t => t.Category.Type == ItemCategoryType.CCC).Count() > 0)
            {
                UpdateOrderVoyages(CurrentOrderID, OrderSpecialType.CCC, Status.Normal);
            }

            Order OriginOrder = new Needs.Ccs.Services.Views.Orders2View().Where(t => t.ID == OriginOrderID).FirstOrDefault();
            if (OriginOrder.Items.Where(t => t.Category.Type == ItemCategoryType.HighValue).Count() == 0)
            {
                UpdateOrderVoyages(OriginOrderID, OrderSpecialType.HighValue, Status.Delete);
            }
            if (OriginOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Inspection).Count() == 0)
            {
                UpdateOrderVoyages(OriginOrderID, OrderSpecialType.Inspection, Status.Delete);
            }
            if (OriginOrder.Items.Where(t => t.Category.Type == ItemCategoryType.Quarantine).Count() == 0)
            {
                UpdateOrderVoyages(OriginOrderID, OrderSpecialType.Quarantine, Status.Delete);
            }
            if (OriginOrder.Items.Where(t => t.Category.Type == ItemCategoryType.CCC).Count() == 0)
            {
                UpdateOrderVoyages(OriginOrderID, OrderSpecialType.CCC, Status.Delete);
            }
        }

        private void UpdateOrderVoyages(string OrderID, OrderSpecialType Type, Status status)
        {
            var order = new Order
            {
                ID = OrderID
            };
            OrderVoyage orderVoyage = new OrderVoyage();
            orderVoyage.Order = order;
            orderVoyage.Type = Type;
            orderVoyage.Status = status;
            if (status == Status.Normal)
            {
                orderVoyage.Enter();
            }
            else
            {
                orderVoyage.Abandon();
            }

        }

        /// <summary>
        /// 拆分后调整订单费用，没拆之前OrderItemA有费用，记录在OrderPremiums表中的OrderID是-01
        /// 拆分之后，OrderItemA的订单是-02，但是OrderPremiums中的OrderID没改
        /// </summary>
        /// <param name="CurrentOrderID"></param>
        private void AdjustOrderFee(string CurrentOrderID)
        {
            Order CurrentOrder = new Needs.Ccs.Services.Views.Orders2View().Where(t => t.ID == CurrentOrderID).FirstOrDefault();
            var OrderItemIDs = CurrentOrder.Items.Select(t => t.ID).ToList();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var orderItemID in OrderItemIDs)
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Where(t => t.OrderItemID == orderItemID).Count();
                    if (count > 0)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderPremiums>(new { OrderID = CurrentOrderID }, t => t.OrderItemID == orderItemID);
                    }
                }
            }
        }

        /// <summary>
        /// 拆分之后调整订单变更，没拆之前OrderItemA有关税变更，然后在OrderChangeNotices表中的OrderID是-01
        /// 拆分之后，OrderItemA的订单是-02，然后在OrderChangeNotices表中的OrderID是
        /// </summary>
        /// <param name="CurrentOrderID"></param>
        private void AdjustOrderChangeNotices(string CurrentOrderID, List<Models.OrderItemAssitant> OrderItems)
        {
            List<string> orderItemIDs = OrderItems.Where(t => t.PersistenceType == PersistenceType.Update).Select(t => t.ID).ToList();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var changedOrderItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == CurrentOrderID && orderItemIDs.Contains(t.ID)).Select(t => t.ID).ToList();
                foreach (var item in changedOrderItem)
                {
                    var orderChangeNoticeLog = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs>().Where(t => t.OrderItemID == item).FirstOrDefault();
                    if (orderChangeNoticeLog != null)
                    {
                        if (orderChangeNoticeLog.OrderID.Trim() != CurrentOrderID.Trim())
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs>(new { OrderID = CurrentOrderID }, t => t.ID == orderChangeNoticeLog.ID);
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(new { OderID = CurrentOrderID }, t => t.ID == orderChangeNoticeLog.OrderChangeNoticeID);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 订单拆分了，付汇申请也要拆分
        /// </summary>
        /// <param name="">需要处理的付汇申请项ID</param>
        private void AdjustPayExchange(string PayExchangeSplit)
        {
            if (string.IsNullOrEmpty(PayExchangeSplit))
            {
                return;
            }

            var pays = PayExchangeSplit.Trim(',').Split(',');

            //拆分金额02:
            var currentTotal02 = CurrentOrder.DeclarePrice;
            //订单金额01(原订单总金额):
            var originTotal01 = OriginOrder.DeclarePrice;
            //已付汇总金额：
            var originPaidPEAmount = OriginOrder.PaidExchangeAmount;
            //需要给02的总金额
            var needSplitAmount = originPaidPEAmount - (originTotal01 - currentTotal02);

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (pays.Length == 1)
                {
                    #region 1、只有一个付汇申请
                    var PEItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().FirstOrDefault(t => t.ID == pays[0]);

                    //修改原item付汇金额 01订单
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Amount = (originPaidPEAmount - needSplitAmount) }, t => t.ID == PEItem.ID);

                    //插入payexchangeitem  02订单
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                        PayExchangeApplyID = PEItem.PayExchangeApplyID,
                        OrderID = CurrentOrder.ID,
                        Amount = needSplitAmount,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        ApplyStatus = PEItem.ApplyStatus
                    });
                    #endregion
                }
                else
                {
                    #region 2、有多个。循环给02，最后一个使用减法给02。

                    var PEItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(t => pays.Contains(t.ID));

                    var last = PEItems.Select(t=>t.ID).ToArray().Last();
                    var total = 0M;
                    foreach (var item in PEItems)
                    {
                        //非最后一个，直接分给02
                        if (item.ID != last)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { OrderID = CurrentOrder.ID }, t => t.ID == item.ID);
                        }
                        else
                        {
                            //最后一个使用剑法，给02，剩余继续给01
                            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                                PayExchangeApplyID = item.PayExchangeApplyID,
                                OrderID = CurrentOrder.ID,
                                Amount = needSplitAmount - total,
                                Status = (int)Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                ApplyStatus = item.ApplyStatus
                            });

                            //修改原item付汇金额 01订单
                            var remain = item.Amount - (needSplitAmount - total);
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Amount = remain }, t => t.ID == item.ID);
                        }

                        total += item.Amount;
                    }

                    #endregion
                }

                //修改01 02订单的已付汇金额
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { PaidExchangeAmount = (OriginOrder.PaidExchangeAmount - needSplitAmount) }, t => t.ID == OriginOrder.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { PaidExchangeAmount = needSplitAmount }, t => t.ID == CurrentOrder.ID);
            }
        }
    }
}
