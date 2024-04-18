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
    /// 根据到货信息，确认订单
    /// </summary>
    public class ConfirmOrder : Order
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
        /// 更新OrderItem的ClassifyStatus
        /// </summary>
        public event OrderDeliveryConfirmedEventHanlder UpdateClassifyStatusEvent;
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

        public ConfirmOrder(List<CgDeliveriesTopViewModel> selectedItems, Order originOrder, List<string> unMatchedOrderItemIDs)
        {
            this.QtyChangedItems = new List<OrderItemChangeCauseOrderChange>();
            this.OrderChangeItems = new List<OrderItemChangeCauseOrderChange>();
            this.SelectedItems = selectedItems;
            this.OriginOrder = originOrder;
            this.UnMatchedOrderItemIDs = unMatchedOrderItemIDs;

            this.OrderItemPersistenceEvent += ItemEnter;
            this.UpdateClassifyStatusEvent += UpdataOrderItemClassifyStatus;
            this.OrderChangeEvent += OrderChange;
            this.AutoClassifyEvent += OrderChangeAutoClassify;
            this.ProductChangeEvent += ProductChange;
            this.CopyClassifyResultEvent += CopyClassifyResult;
            this.Post2WareHouseEvent += Post2Warehouse;
            this.PostCurrentOrderInfoEvent += PostCurrentOrderInfo;
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

            if (p.Batch != originItem.Batch && p.Batch != null && p.Batch != "")
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
        private void OnConfirmed(List<OrderItemAssitant> ItemAssistants)
        {
            this.OrderItemPersistenceEvent(this, new OrderDeliveryConfirmedEventArgs(ItemAssistants));
            UpdateOrderDeclareFlag();
            DeleteModel();
            ChangeQuantity();
            OrderChange();
        }
        private void ItemEnter(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                foreach (var orderitem in e.OrderItems)
                {
                    switch (orderitem.PersistenceType)
                    {
                        case PersistenceType.Update:
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                            {
                                Origin = orderitem.Origin,
                                Quantity = orderitem.Quantity,
                                TotalPrice = orderitem.TotalPrice,
                                Name = orderitem.Name,
                                Model = orderitem.Model,
                                Manufacturer = orderitem.Manufacturer,
                                Batch = orderitem.Batch
                            }, item => item.ID == orderitem.ID);
                            break;

                        case PersistenceType.Insert:
                            var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                            OrderItem newOrderItem = new OrderItem();
                            string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                            orderitem.ID = singleOrderItemID;
                            newOrderItem.ID = singleOrderItemID;
                            newOrderItem.OrderID = OriginOrder.ID;
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
                            newOrderItem.Batch = orderitem.Batch;
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
                    orderItemChangeNotice.Sorter = OriginOrder.Client.Merchandiser;

                    orderItemChangeNotice.Enter();

                    OrderItemChangeLog orderItemChangeLog = new OrderItemChangeLog();
                    orderItemChangeLog.OrderID = OriginOrder.ID;
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
            try
            {
                string orderID = OriginOrder.ID;
                string currency = OriginOrder.Currency;
                List<Post2WarehouseModel> PostData = new List<Post2WarehouseModel>();
                foreach (var item in e.OrderItems)
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
                    TinyOrderID = OriginOrder.ID,
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
                ex.CcsLog("到货确认调用库房接口传当前跟单确认到货信息");
            }

        }
        private void PostCurrentOrderInfo(object sender, OrderDeliveryConfirmedEventArgs e)
        {
            try
            {
                List<Needs.Ccs.Services.Views.OrderItemChanges> listOrderItemChanges = new List<Needs.Ccs.Services.Views.OrderItemChanges>();
                foreach (var item in e.OrderItems)
                {
                    if (item.InputID.Contains(','))
                    {
                        string[] inputids = item.InputID.Split(',');
                        //foreach (var t in inputids)
                        //{
                            //var qty = SelectedItems.Where(p => p.ID == t).FirstOrDefault();
                            listOrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                            {
                                OrderItemID = item.ID,
                                //InputID = t,
                                InputID = inputids[0],
                                CustomName = item.Name,
                                Product = new Needs.Ccs.Services.Views.CenterProduct()
                                {
                                    PartNumber = item.Model,
                                    Manufacturer = item.Manufacturer,
                                },
                                Origin = item.Origin,
                                DateCode = item.Batch,
                                //Quantity = qty == null ? 0 : qty.Quantity,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                Unit = item.Unit,
                                TinyOrderID = OriginOrder.ID,
                            });
                        //}
                    }
                    else
                    {
                        listOrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                        {
                            OrderItemID = item.ID,
                            InputID = item.InputID,
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
                            TinyOrderID = OriginOrder.ID,
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
                    TinyOrderID = OriginOrder.ID,
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
            catch (Exception ex)
            {
                ex.CcsLog("到货确认接口中调用代仓储接口传当前跟单确认到货信息");
            }

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

        /// <summary>
        /// 修改数量
        /// 删除型号
        /// 新增型号
        /// 重新归类引起的关税，增值税变更，不在这里插入OrderChange,是在重新归类那里插入OrderChange
        /// </summary>
        private void OrderChange()
        {
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

            if (this.OrderChangeItems.Count() > 0)
            {
                OrderChangeNotice orderChangeNotice = new OrderChangeNotice();
                orderChangeNotice.ID = ChainsGuid.NewGuidUp();
                orderChangeNotice.OrderID = OriginOrder.ID;
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
                    orderChangeNoticeLog.OrderID = OriginOrder.ID;
                    orderChangeNoticeLog.OrderItemID = orderItem.OriginalOrderItemID;
                    orderChangeNoticeLog.AdminID = admin.ID;
                    orderChangeNoticeLog.CreateDate = DateTime.Now;

                    switch (orderItem.ReasonType)
                    {
                        case OrderChangeCasuedReason.AddOrderItem:
                            orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,新增了型号【" + orderItem.OriginalModel + "】";
                            break;

                        case OrderChangeCasuedReason.DeleteOrderItem:
                            orderChangeNoticeLog.Summary = "跟单员【" + admin.RealName + "】,删除了型号【" + orderItem.OriginalModel + "】";
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
    }
}
