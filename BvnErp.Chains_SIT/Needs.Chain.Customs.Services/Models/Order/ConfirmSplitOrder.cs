using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 根据到货，拆分订单
    /// </summary>
    public class ConfirmSplitOrder : Order
    {
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

        public ConfirmSplitOrder(List<CgDeliveriesTopViewModel> selectedItems, Order originOrder)
        {
            this.QtyChangedItems = new List<OrderItemChangeCauseOrderChange>();
            this.OrderChangeItems = new List<OrderItemChangeCauseOrderChange>();
            this.SelectedItems = selectedItems;
            this.OriginOrder = originOrder;

            this.OrderItemPersistenceEvent += ItemEnter;

            this.OrderChangeEvent += OrderChange;
            this.AutoClassifyEvent += OrderChangeAutoClassify;

            this.ProductChangeEvent += ProductChange;
            this.CopyClassifyResultEvent += CopyClassifyResult;

            this.UpdateClassifyStatusEvent += UpdataOrderItemClassifyStatus;
            this.Post2WareHouseEvent += Post2Warehouse;
            this.PostCurrentOrderInfoEvent += PostCurrentOrderInfo;
            this.Post2ClientEvent += Post2ClientConfirm;
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
        /// <summary>
        /// 更新OrderItem的ClassifyStatus
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder UpdateClassifyStatusEvent;

        public event OrderDeliveryConfirmedEventHanlder Post2WareHouseEvent;
        public event OrderDeliveryConfirmedEventHanlder PostCurrentOrderInfoEvent;
        public event OrderDeliveryConfirmedEventHanlder Post2ClientEvent;

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
                                assistant.OrderID = this.ID;
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
                                assistant.OrderID = this.ID;
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

            if (p.Batch != originItem.Batch)
            {
                OrderItemChange change = new OrderItemChange();
                change.OrderItemChangeType = Enums.OrderItemChangeType.BatchChange;
                change.OldValue = originItem.Batch;
                change.NewValue = p.Batch;

                assistant.OrderItemChanges.Add(change);
                assistant.ChangeType = Enums.MatchChangeType.ProductChange;
            }

            return assistant;
        }

        public void OnSplited(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderItemPersistenceEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            ChangeQuantity();
            OrderChange();
        }

        private void ItemEnter(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                //新增一条Order记录   
                this.DeclareFlag = Enums.DeclareFlagEnums.Able;
                reponsitory.Insert(this.ToLinq());
                reponsitory.Insert(this.OrderConsignee.ToLinq());
                reponsitory.Insert(this.OrderConsignor.ToLinq());
                foreach (var payExchangeSupplier in this.PayExchangeSuppliers)
                {
                    reponsitory.Insert(payExchangeSupplier.ToLinq());
                }

                foreach (var orderitem in e.OrderItems)
                {
                    switch (orderitem.PersistenceType)
                    {
                        case PersistenceType.Update:
                            if (!string.IsNullOrEmpty(orderitem.OrderID))
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                                {
                                    OrderID = orderitem.OrderID,
                                    Quantity = orderitem.Quantity,
                                    TotalPrice = orderitem.TotalPrice,
                                    Name = orderitem.Name,
                                    Model = orderitem.Model,
                                    Manufacturer = orderitem.Manufacturer,
                                    Batch = orderitem.Batch
                                }, item => item.ID == orderitem.ID);
                            }
                            else
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                                {
                                    Quantity = orderitem.Quantity,
                                    TotalPrice = orderitem.TotalPrice
                                }, item => item.ID == orderitem.ID);
                            }
                            break;

                        case PersistenceType.Insert:
                            var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                            OrderItem newOrderItem = new OrderItem();
                            string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                            orderitem.ID = singleOrderItemID;
                            newOrderItem.ID = singleOrderItemID;
                            newOrderItem.OrderID = this.ID;
                            newOrderItem.Origin = orderitem.Origin;
                            newOrderItem.Quantity = orderitem.Quantity;
                            newOrderItem.Unit = orderitem.Unit;
                            newOrderItem.UnitPrice = orderitem.UnitPrice;
                            newOrderItem.TotalPrice = orderitem.TotalPrice;
                            newOrderItem.IsSampllingCheck = false;
                            newOrderItem.ClassifyStatus = Enums.ClassifyStatus.Unclassified;
                            newOrderItem.Status = Enums.Status.Normal;
                            newOrderItem.CreateDate = DateTime.Now;
                            newOrderItem.UpdateDate = DateTime.Now;
                            newOrderItem.Name = orderitem.Name;
                            newOrderItem.Model = orderitem.Model;
                            newOrderItem.Manufacturer = orderitem.Manufacturer;
                            reponsitory.Insert(newOrderItem.ToLinq());
                            break;

                        default:
                            break;
                    }
                }

                reponsitory.Submit();
            }
            OnPersistenced(e.OrderItems);
        }

        public void OnPersistenced(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderChangeEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.AutoClassifyEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.ProductChangeEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.CopyClassifyResultEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.Post2Warehouse(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.PostCurrentOrderInfo(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            this.Post2ClientEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
        }

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
        }

        //自动归类，生成订单变更
        private void OrderChangeAutoClassify(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            //只有订单变更 新增的OrderItem 才需要自动归类
            var orderItems = e.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.OrderChange).ToList();

            var pvdataApi = new PvDataApiSetting();
            string PvDataApiUrl = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.AutoClassify;


            foreach (var t in orderItems)
            {
                try
                {
                    var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JSingle<dynamic>>(PvDataApiUrl, new
                    {
                        partnumber = t.Model,
                        manufacturer = t.Manufacturer,
                        unitPrice = t.UnitPrice,
                        isVerifyPriceFluctuation = true,
                        highPriceLimit = 0.8,
                        lowPriceLimit = 0.3,
                        origin = t.Origin
                    });
                    if (result.code == 200)
                    {
                        #region OrderItemCategory
                        OrderItemCategory category = new OrderItemCategory();
                        category.ID = string.Concat(t.ID).MD5();
                        category.OrderItemID = t.ID;
                        category.TaxCode = result.data.TaxCode;
                        category.TaxName = result.data.TaxName;
                        category.HSCode = result.data.HSCode;
                        category.Name = result.data.TariffName;
                        category.Elements = result.data.Elements;
                        category.Unit1 = result.data.LegalUnit1;
                        category.Unit2 = result.data.LegalUnit2;
                        category.CIQCode = result.data.CIQCode;
                        category.ClassifyFirstOperatorID = Icgoo.DefaultCreator;
                        category.Status = Enums.Status.Normal;
                        category.CreateDate = DateTime.Now;
                        category.UpdateDate = DateTime.Now;

                        category.Type = ItemCategoryType.Normal;
                        if (result.data.CIQ)
                        {
                            category.Type |= ItemCategoryType.Inspection;
                        }
                        if (result.data.Ccc)
                        {
                            category.Type |= ItemCategoryType.CCC;
                        }
                        if (result.data.Coo)
                        {
                            category.Type |= ItemCategoryType.OriginProof;
                        }
                        if (result.data.IsSysEmbargo || result.data.Embargo)
                        {
                            category.Type |= ItemCategoryType.Forbid;
                        }
                        if (result.data.HkControl)
                        {
                            category.Type |= ItemCategoryType.HKForbid;
                        }
                        if (result.data.IsHighPrice)
                        {
                            category.Type |= ItemCategoryType.HighValue;
                        }
                        if (result.data.IsDisinfected)
                        {
                            category.Type |= ItemCategoryType.Quarantine;
                        }


                        category.Enter();
                        #endregion

                        #region
                        //5.添加订单项关税、增值税、消费税记录
                        OrderItemTax itemTax = new OrderItemTax();
                        itemTax.ID = string.Concat(t.ID, CustomsRateType.ImportTax).MD5();
                        itemTax.OrderItemID = t.ID;
                        itemTax.Type = CustomsRateType.ImportTax;
                        itemTax.Rate = result.data.ImportPreferentialTaxRate + result.data.OriginRate;
                        itemTax.Value = Math.Round(t.TotalPrice * itemTax.Rate, 2, MidpointRounding.AwayFromZero);
                        itemTax.Status = Status.Normal;
                        itemTax.CreateDate = DateTime.Now;
                        itemTax.UpdateDate = DateTime.Now;
                        itemTax.Enter();

                        OrderItemTax itemAddedValue = new OrderItemTax();
                        itemAddedValue.ID = string.Concat(t.ID, CustomsRateType.AddedValueTax).MD5();
                        itemAddedValue.OrderItemID = t.ID;
                        itemAddedValue.Type = CustomsRateType.AddedValueTax;
                        itemAddedValue.Rate = result.data.VATRate;
                        itemAddedValue.Value = Math.Round(t.TotalPrice * itemAddedValue.Rate, 2, MidpointRounding.AwayFromZero);
                        itemAddedValue.Status = Status.Normal;
                        itemAddedValue.CreateDate = DateTime.Now;
                        itemAddedValue.UpdateDate = DateTime.Now;
                        itemAddedValue.Enter();

                        OrderItemTax exciseTax = new OrderItemTax();
                        exciseTax.ID = string.Concat(t.ID, CustomsRateType.ConsumeTax).MD5();
                        exciseTax.OrderItemID = t.ID;
                        exciseTax.Type = CustomsRateType.ConsumeTax;
                        exciseTax.Rate = result.data.ExciseTaxRate;
                        exciseTax.Value = Math.Round(t.TotalPrice * exciseTax.Rate, 2, MidpointRounding.AwayFromZero);
                        exciseTax.Status = Status.Normal;
                        exciseTax.CreateDate = DateTime.Now;
                        exciseTax.UpdateDate = DateTime.Now;
                        exciseTax.Enter();
                        #endregion

                        #region 商检
                        //6.商检
                        if ((category.Type & ItemCategoryType.Inspection) > 0)
                        {
                            var premium = new OrderPremium
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                                OrderID = this.ID,
                                OrderItemID = t.ID,
                                Type = Enums.OrderPremiumType.InspectionFee,
                                Count = 1,
                                UnitPrice = result.data.CIQprice,
                                Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY),
                                Rate = 1,
                                Admin = new Admin
                                {
                                    ID = Icgoo.DefaultCreator
                                }
                            };
                            premium.Enter();
                        }
                        #endregion

                        t.ClassifyStatus = ClassifyStatus.Done;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            OnClassified(orderItems);
        }

        private void ProductChange(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var orderItems = e.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.ProductChange);
            foreach (var t in orderItems)
            {
                foreach (var p in t.OrderItemChanges)
                {
                    OrderItemChangeNotice orderItemChangeNotice = new OrderItemChangeNotice();
                    orderItemChangeNotice.Type = p.OrderItemChangeType;
                    orderItemChangeNotice.TriggerSource = TriggerSource.CheckDecListMan;
                    orderItemChangeNotice.OrderItemID = t.ID;
                    orderItemChangeNotice.ProcessState = ProcessState.UnProcess;
                    orderItemChangeNotice.Status = Status.Normal;
                    orderItemChangeNotice.CreateDate = DateTime.Now;
                    orderItemChangeNotice.UpdateDate = DateTime.Now;
                    orderItemChangeNotice.OldValue = p.OldValue;
                    orderItemChangeNotice.NewValue = p.NewValue;
                    orderItemChangeNotice.IsSplited = false;

                    orderItemChangeNotice.Enter();

                    OrderItemChangeLog orderItemChangeLog = new OrderItemChangeLog();
                    orderItemChangeLog.OrderID = this.ID;
                    orderItemChangeLog.OrderItemID = t.ID;
                    orderItemChangeLog.Admin = OriginOrder.Client.Merchandiser;
                    orderItemChangeLog.Type = p.OrderItemChangeType;
                    orderItemChangeLog.Summary = "跟单员[" + OriginOrder.Client.Merchandiser.RealName + "]做了" + p.OrderItemChangeType.GetDescription() + "操作,从[" + p.OldValue + "]变更为[" + p.NewValue + "]";
                    orderItemChangeLog.Enter();
                }
            }
        }

        private void CopyClassifyResult(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var orderItems = e.OrderItems.Where(t => t.ChangeType == Enums.MatchChangeType.ProductChange && t.PersistenceType == PersistenceType.Insert).ToList();
            foreach (var t in orderItems)
            {
                try
                {
                    var sourceClassifyResult = OriginOrder.Items.Where(m => m.ID == t.MatchedOrderItemID).FirstOrDefault();

                    #region OrderItemCategory
                    OrderItemCategory category = new OrderItemCategory();
                    category.ID = string.Concat(t.ID).MD5();
                    category.OrderItemID = t.ID;
                    category.TaxCode = sourceClassifyResult.Category.TaxCode;
                    category.TaxName = sourceClassifyResult.Category.TaxName;
                    category.HSCode = sourceClassifyResult.Category.HSCode;
                    category.Name = sourceClassifyResult.Category.Name;
                    category.Elements = sourceClassifyResult.Category.Elements;
                    category.Unit1 = sourceClassifyResult.Category.Unit1;
                    category.Unit2 = sourceClassifyResult.Category.Unit2;
                    category.CIQCode = sourceClassifyResult.Category.CIQCode;
                    category.ClassifyFirstOperatorID = Icgoo.DefaultCreator;
                    category.ClassifySecondOperatorID = Icgoo.DefaultCreator;
                    category.Status = Enums.Status.Normal;
                    category.CreateDate = DateTime.Now;
                    category.UpdateDate = DateTime.Now;

                    category.Type = sourceClassifyResult.Category.Type;

                    category.Enter();
                    #endregion

                    #region
                    //5.添加订单项关税、增值税、消费税记录
                    OrderItemTax itemTax = new OrderItemTax();
                    itemTax.ID = string.Concat(t.ID, CustomsRateType.ImportTax).MD5();
                    itemTax.OrderItemID = t.ID;
                    itemTax.Type = CustomsRateType.ImportTax;
                    itemTax.Rate = sourceClassifyResult.ImportTax.Rate;
                    itemTax.Value = Math.Round(t.TotalPrice * itemTax.Rate, 2, MidpointRounding.AwayFromZero);
                    itemTax.Status = Status.Normal;
                    itemTax.CreateDate = DateTime.Now;
                    itemTax.UpdateDate = DateTime.Now;
                    itemTax.Enter();

                    OrderItemTax itemAddedValue = new OrderItemTax();
                    itemAddedValue.ID = string.Concat(t.ID, CustomsRateType.AddedValueTax).MD5();
                    itemAddedValue.OrderItemID = t.ID;
                    itemAddedValue.Type = CustomsRateType.AddedValueTax;
                    itemAddedValue.Rate = sourceClassifyResult.AddedValueTax.Rate;
                    itemAddedValue.Value = Math.Round(t.TotalPrice * itemAddedValue.Rate, 2, MidpointRounding.AwayFromZero);
                    itemAddedValue.Status = Status.Normal;
                    itemAddedValue.CreateDate = DateTime.Now;
                    itemAddedValue.UpdateDate = DateTime.Now;
                    itemAddedValue.Enter();

                    if (sourceClassifyResult.ExciseTax != null)
                    {
                        OrderItemTax exciseTax = new OrderItemTax();
                        exciseTax.ID = string.Concat(t.ID, CustomsRateType.ConsumeTax).MD5();
                        exciseTax.OrderItemID = t.ID;
                        exciseTax.Type = CustomsRateType.ConsumeTax;
                        exciseTax.Rate = sourceClassifyResult.ExciseTax.Rate;
                        exciseTax.Value = Math.Round(t.TotalPrice * exciseTax.Rate, 2, MidpointRounding.AwayFromZero);
                        exciseTax.Status = Status.Normal;
                        exciseTax.CreateDate = DateTime.Now;
                        exciseTax.UpdateDate = DateTime.Now;
                        exciseTax.Enter();
                    }
                    #endregion

                    #region 商检
                    //6.商检
                    if ((category.Type & ItemCategoryType.Inspection) > 0)
                    {
                        var InspectionFee = OriginOrder.Premiums.Where(m => m.Type == OrderPremiumType.InspectionFee && m.OrderItemID == t.ID).FirstOrDefault();
                        if (InspectionFee != null)
                        {
                            var premium = new OrderPremium
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                                OrderID = this.ID,
                                OrderItemID = t.ID,
                                Type = Enums.OrderPremiumType.InspectionFee,
                                Count = 1,
                                UnitPrice = InspectionFee.UnitPrice,
                                Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY),
                                Rate = 1,
                                Admin = new Admin
                                {
                                    ID = Icgoo.DefaultCreator
                                }
                            };
                            premium.Enter();
                        }
                    }
                    #endregion

                    t.ClassifyStatus = ClassifyStatus.Done;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            OnClassified(orderItems);
        }

        private void OnClassified(List<OrderItemAssitant> ItemAssistants)
        {
            this.UpdateClassifyStatusEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
        }
        private void UpdataOrderItemClassifyStatus(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            var items = e.OrderItems.Where(t => t.ClassifyStatus == Enums.ClassifyStatus.Done);
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in items)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                    {
                        ClassifyStatus = (int)Enums.ClassifyStatus.Done
                    }, t => t.ID == item.ID);
                }
            }
        }

        private void Post2Warehouse(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            try
            {
                string orderID = this.ID;
                string currency = OriginOrder.Currency;
                List<Post2WarehouseModel> PostData = new List<Post2WarehouseModel>();
                foreach (var item in e.OrderItems.Where(t=> !string.IsNullOrEmpty(t.InputID)))
                {
                    if (item.InputID.Contains(','))
                    {
                        string[] inputids = item.InputID.Split(',');
                        foreach (var t in inputids)
                        {
                            Post2WarehouseModel post = new Post2WarehouseModel();
                            post.Unique = t;
                            post.ItemID = item.ID;
                            post.TinyOrderID = orderID;
                            post.Currency = currency;
                            post.Price = item.UnitPrice;
                            PostData.Add(post);
                        }
                    }
                    else
                    {
                        Post2WarehouseModel post = new Post2WarehouseModel();
                        post.Unique = item.InputID;
                        post.ItemID = item.ID;
                        post.TinyOrderID = orderID;
                        post.Currency = currency;
                        post.Price = item.UnitPrice;
                        PostData.Add(post);
                    }
                }

                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                string address = apisetting.UpdateInput;
                if (PostData.FirstOrDefault().Unique.Substring(0, 1).ToLower().Equals("o"))
                {
                    address = apisetting.UpdateOutput;
                }
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + address;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = OriginOrder.MainOrderID,
                    TinyOrderID = this.ID,
                    Url = apiurl,
                    RequestContent = PostData.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();

                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, PostData);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("到货拆分调用库房接口传当前跟单确认到货信息");
            }

        }       
        private void ChangeQuantity()
        {
            if (this.QtyChangedItems.Count() > 0)
            {
                ArrivalChangeQuantityHandler changeQuantityHandler = new ArrivalChangeQuantityHandler(this.QtyChangedItems, this.ID);
                bool result = changeQuantityHandler.Execute();
                if (result)
                {
                    try
                    {
                        if (changeQuantityHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                        {
                            var bill = OriginOrder.MainOrderFiles.Where(file => file.FileType == FileType.OrderBill && file.Status == Status.Normal).SingleOrDefault();

                            bill?.Abandon();
                            this.GenerateBill();
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
        /// 修改数量
        /// 删除型号 这里不会有删除型号
        /// 新增型号
        /// 重新归类引起的关税，增值税变更，不在这里插入OrderChange,是在重新归类那里插入OrderChange
        /// </summary>
        private void OrderChange()
        {
            if (this.OrderChangeItems.Count > 0)
            {
                OrderChangeNotice orderChangeNotice = new OrderChangeNotice();
                orderChangeNotice.ID = ChainsGuid.NewGuidUp();
                orderChangeNotice.OrderID = this.ID;
                orderChangeNotice.Type = OrderChangeType.ArrivalChange;
                orderChangeNotice.processState = ProcessState.Processing;
                orderChangeNotice.Status = Status.Normal;
                orderChangeNotice.CreateDate = DateTime.Now;
                orderChangeNotice.UpdateDate = DateTime.Now;
                orderChangeNotice.Enter();

                var admin = OriginOrder.Client.Merchandiser;

                foreach (var orderItem in this.OrderChangeItems)
                {
                    OrderChangeNoticeLog orderChangeNoticeLog = new OrderChangeNoticeLog();
                    orderChangeNoticeLog.ID = ChainsGuid.NewGuidUp();
                    orderChangeNoticeLog.OrderChangeNoticeID = orderChangeNotice.ID;
                    orderChangeNoticeLog.OrderID = this.ID;
                    orderChangeNoticeLog.OrderItemID = orderItem.OriginalOrderItemID;
                    orderChangeNoticeLog.AdminID = admin.ID;
                    orderChangeNoticeLog.CreateDate = DateTime.Now;

                    switch (orderItem.ReasonType)
                    {
                        case OrderChangeCasuedReason.AddOrderItem:
                            orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,新增了型号【" + orderItem.OriginalModel + "】,产地【"+orderItem.Origin+"】";
                            break;

                        case OrderChangeCasuedReason.ChangeQty:
                            orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,修改了型号【" + orderItem.OriginalModel + "】,数量【" + orderItem.OriginalQty + "】改为【" + orderItem.NowQty + "】";
                            break;

                        default:
                            orderChangeNoticeLog.Summary = "";
                            break;
                    }

                    orderChangeNoticeLog.Enter();
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
                //新建拆分的订单时，订单的初始状态为待归类，在这个情况下，什么都不需要改，可以直接报关，订单状态需改为待报关
                UpdateSplitOrderStatus();
            }
        }

        private void Post2Client()
        {
            try
            {
                var Orders = new Views.Orders2View().Where(item => item.MainOrderID == this.MainOrderID &&
                                                          item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled &&
                                                          item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned).ToList();

                List<TinyOrderDeclareFlags> declareFlags = new List<TinyOrderDeclareFlags>();

                foreach (var order in Orders)
                {
                    if (order.ID != this.ID)
                    {
                        TinyOrderDeclareFlags tinyOrder = new TinyOrderDeclareFlags();
                        tinyOrder.TinyOrderID = order.ID;
                        tinyOrder.IsDeclare = false;
                        if (order.DeclareFlag == Needs.Ccs.Services.Enums.DeclareFlagEnums.Able)
                        {
                            tinyOrder.IsDeclare = true;
                        }
                        declareFlags.Add(tinyOrder);
                    }
                }

                TinyOrderDeclareFlags thistinyOrder = new TinyOrderDeclareFlags();
                thistinyOrder.TinyOrderID = this.ID;
                thistinyOrder.IsDeclare = true;
                declareFlags.Add(thistinyOrder);

                var ermAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == this.Client.Merchandiser.ID)?.ID;

                var confirm = new ClientConfirm();
                confirm.OrderID = this.MainOrderID;
                confirm.AdminID = ermAdminID;
                confirm.Type = ConfirmType.DirectConfirm;
                confirm.DeclareFlags = declareFlags;

                var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ClientConfirm;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = this.MainOrderID,
                    TinyOrderID = this.ID,
                    Url = apiurl,
                    RequestContent = confirm.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();


                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, confirm);
                apiLog.ResponseContent = result;
                apiLog.Enter();

                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Needs.Underly.JMessage>(result);

                if (message.code != 200)
                {
                    OrderLog log = new OrderLog();
                    log.OrderID = this.ID;
                    //log.Admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
                    log.OrderStatus = Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed;
                    log.Summary = "到货异常处理推送代仓储失败:" + message.data;
                    log.Enter();
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void UpdateSplitOrderStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    OrderStatus = (int)Enums.OrderStatus.QuoteConfirmed
                }, t => t.ID == this.ID);
            }
        }

        /// <summary>
        /// 拆分报关给代仓储的时候，要给所有OrderItem信息，不能只给选定的OrderItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostCurrentOrderInfo(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            try
            {
                List<Needs.Ccs.Services.Views.OrderItemChanges> listOrderItemChanges = new List<Needs.Ccs.Services.Views.OrderItemChanges>();

                var cgDeliveryView = new Needs.Ccs.Services.Views.CgDeliveriesTopViewOrigin().
                   Where(item => item.MainOrderID == this.MainOrderID && item.OrderItemID != null).OrderBy(item => item.CaseNo).ToList();

                var OrderIDs = new Views.Orders2View().Where(item => item.MainOrderID == this.MainOrderID).Select(item => item.ID).ToList();
                foreach (var orderId in OrderIDs)
                {
                    var order = new Views.Orders2View().Where(t => t.ID == orderId).FirstOrDefault();
                    foreach (var item in order.Items)
                    {
                        var deliveryInfo = cgDeliveryView.Where(t => t.OrderItemID == item.ID).FirstOrDefault();
                        listOrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                        {
                            OrderItemID = item.ID,
                            InputID = deliveryInfo == null ? "" : deliveryInfo.ID,
                            CustomName = item.Name,
                            Product = new Needs.Ccs.Services.Views.CenterProduct()
                            {
                                PartNumber = item.Model,
                                Manufacturer = item.Manufacturer,
                            },
                            Origin = item.Origin,
                            DateCode = item.Batch,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice,
                            Unit = item.Unit,
                            TinyOrderID = this.ID,
                        });
                    }
                }

                Needs.Ccs.Services.Views.CurrentOrderInfo currentOrderInfo = new Needs.Ccs.Services.Views.CurrentOrderInfo()
                {
                    OrderID = OriginOrder.MainOrderID,
                    Currency = OriginOrder.Currency,
                    Confirmed = false,
                    items = listOrderItemChanges,
                };

                var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitChanged;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = OriginOrder.MainOrderID,
                    TinyOrderID = this.ID,
                    Url = apiurl,
                    RequestContent = currentOrderInfo.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();

                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, currentOrderInfo);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch(Exception ex)
            {
                ex.CcsLog("到货拆分接口中调用代仓储接口传当前跟单确认到货信息");
            }          
        }
    }
}
