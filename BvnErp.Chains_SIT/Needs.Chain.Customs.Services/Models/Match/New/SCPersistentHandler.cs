using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 新订单，订单项持久化
    /// </summary>
    public class SCPersistentHandler : SCHandler
    {
        public SCPersistentHandler(List<MatchViewModel> selectedItems, Order originOrder, Order currentOrder)
        {
            SelectedItems = selectedItems;
            OriginOrder = originOrder;
            CurrentOrder = currentOrder;
        }

        /// <summary>
        /// 拆分订单，持久化要做的事情：
        /// 1、插入新的Order、OrderConsignee、OrderConsignor、payExchangeSupplier
        /// 2、插入新的EntryNotices
        /// 3、已有的OrderItem且数量与原来的OrderItem相等
        ///    - 改OrderItems表中的OrderID
        ///    - 根据OrderItemID 更改EntryNoticeItems 中的EntryNoticeID 为新增的EntryNoticeID
        ///    - 根据OrderItemID 更改Sortings 中的OrderID 为新增的OrderID
        ///    - 根据OrderItemID 在Sortings 里查到SortingID,根据SoringID 在PackingItems 找到PackingID ,更改Packings里的OrderID为新的OrderID
        ///    
        /// 4、已有的OrderItem且数量与原来的OrderItem不相等（部分到货，拆分报关）
        ///    - 新增OrderItems 中的项
        ///    - 改原OrderItem 的数量
        ///    - 新增EntryNoticeItems,EntryNoticeID为新增的EntryNoticeID  
        ///    - 根据原OrderItemID、数量、箱号 在Sortings 里查到SortingID，更改Sorting的OrderID为新的OrderID，OrderItemID 为新的OrderItemID
        ///    - 根据SortingID 去PackingItems找PackingID，根据PackingID 改Pakings 里的OrderID为新的OrderID
        /// 
        /// 5、无通知产品录入
        ///    - 新增OrderItems 中的项,
        ///    - 根据oldOrderID 和Model 查询UnExpectedOrderItem
        ///      -> 用箱号和newOrderID 去查询Packing，没有就新增，有就取PackingID
        ///    - 新增EntryNoticeItems,EntryNoticeID为新增的EntryNoticeID  
        ///    - 新增Sorting OrderID:新增的OrderID,OrderItemID:新增的OrderItemID,EntryNoticeItemID:新增的EntryNoticeItemID
        ///                  BoxIndex:根据Model 在去UnExpectedOrderItem找，NetWeight、GrossWeight 不需要自动分配
        ///    - 新增PackingItem，SortingID为新增的SortingID, PackingID 为新增的PackingID           
        ///                  
        /// 6、更改原 Order 中的报关价格
        /// </summary>
        public override void handleRequest()
        {
            try
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

                    //插入新的EntryNotice
                    string EntryNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice); ;
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EntryNotices
                    {
                        ID = EntryNoticeID,
                        OrderID = CurrentOrder.ID,
                        WarehouseType = (int)Enums.WarehouseType.HongKong,
                        ClientCode = CurrentOrder.Client.ClientCode,
                        SortingRequire = (int)Enums.SortingRequire.UnPacking,
                        EntryNoticeStatus = (int)Enums.EntryNoticeStatus.Boxed,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });

                    //3、已有的OrderItem且数量与原来的OrderItem相等 的处理
                    //更改原OrderItemID 的 OrderID 为新的OrderID
                    var OriginalItemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.OrderItemID) && t.Qty == t.OrderItemQty).Select(t => t.OrderItemID).ToList();

                    //3-1、同一个OrderItem，分了两个箱子装,这两个箱子装的数量与订单的数量相等
                    //更改原OrderItemID 的 OrderID 为新的OrderID
                    List<string> splitOrderItemIDs = new List<string>();
                    var OriginalSplitItemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.OrderItemID) && t.Qty != t.OrderItemQty).Select(t => t.OrderItemID).Distinct().ToList();
                    foreach (var splitItem in OriginalSplitItemIDs)
                    {
                        var qty = SelectedItems.Where(t => t.OrderItemID == splitItem).Sum(t => t.Qty);
                        var orderItemqty = SelectedItems.Where(t => t.OrderItemID == splitItem).Select(t => t.OrderItemQty).FirstOrDefault();
                        if (orderItemqty == qty)
                        {
                            OriginalItemIDs.Add(splitItem);
                        }
                    }

                    foreach (var orderItemID in OriginalItemIDs)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { OrderID = CurrentOrder.ID }, t => t.ID == orderItemID);
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(new { EntryNoticeID = EntryNoticeID }, t => t.OrderItemID == orderItemID);
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { OrderID = CurrentOrder.ID }, t => t.OrderItemID == orderItemID);

                        string SortingID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().Where(t => t.OrderItemID == orderItemID && t.Status == (int)Enums.Status.Normal).FirstOrDefault().ID;
                        string packingID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>().Where(t => t.SortingID == SortingID && t.Status == (int)Enums.Status.Normal).FirstOrDefault().PackingID;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { OrderID = CurrentOrder.ID }, t => t.ID == packingID);
                    }

                   
                    // 4、已有的OrderItem且数量与原来的OrderItem不相等（部分到货，拆分报关）
                    // 需要判断一个型号，装在两个箱子的情况
                    var PartArrivedItemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.OrderItemID) && t.Qty != t.OrderItemQty).ToList();
                    foreach (var orderItem in PartArrivedItemIDs)
                    {
                        var packedQty = SelectedItems.Where(t => t.OrderItemID == orderItem.OrderItemID).Sum(t => t.Qty);
                        if(packedQty == orderItem.OrderItemQty)
                        {
                            continue;
                        }
                        var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                        OrderItem newOrderItem = new OrderItem();
                        string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                        orderItem.NewOrderItemID = singleOrderItemID;
                        newOrderItem.ID = singleOrderItemID;
                        newOrderItem.OrderID = CurrentOrder.ID;
                        newOrderItem.Origin = orderItem.Origin;
                        newOrderItem.Quantity = orderItem.Qty;
                        newOrderItem.Unit = orderItem.Unit;
                        newOrderItem.UnitPrice = orderItem.UnitPrice;
                        newOrderItem.TotalPrice = orderItem.TotalPrice;
                        newOrderItem.IsSampllingCheck = false;
                        newOrderItem.ClassifyStatus = Enums.ClassifyStatus.Unclassified;
                        newOrderItem.Status = Enums.Status.Normal;
                        newOrderItem.CreateDate = DateTime.Now;
                        newOrderItem.UpdateDate = DateTime.Now;
                        newOrderItem.Name = orderItem.Name;
                        newOrderItem.Model = orderItem.Model;
                        newOrderItem.Manufacturer = orderItem.Brand;
                        newOrderItem.Batch = orderItem.BatchNo;
                        reponsitory.Insert(newOrderItem.ToLinq());

                        //更改原订单数量，以及总价
                        decimal originQty = orderItem.OrderItemQty - SelectedItems.Where(t => t.OrderItemID == orderItem.OrderItemID).Sum(t => t.Qty);
                        decimal originTotalPrice = orderItem.UnitPrice * originQty;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { Quantity = originQty,TotalPrice = originTotalPrice }, t => t.ID == orderItem.OrderItemID);

                        string EntryNoticeItemID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EntryNoticeItems
                        {
                            ID = EntryNoticeItemID,
                            EntryNoticeID = EntryNoticeID,
                            OrderItemID = singleOrderItemID,
                            EntryNoticeStatus = (int)Enums.EntryNoticeStatus.Boxed,
                            IsSpotCheck = false,
                            Status = (int)Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });

                        string SortingID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().
                                           Where(t => t.OrderItemID == orderItem.OrderItemID && 
                                                      t.Quantity == orderItem.Qty &&
                                                      t.BoxIndex == orderItem.CaseNo&&
                                                      t.Status==(int)Enums.Status.Normal).FirstOrDefault().ID;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { OrderID = CurrentOrder.ID, OrderItemID = singleOrderItemID, EntryNoticeItemID = EntryNoticeItemID }, t => t.ID == SortingID);

                        string packingID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>().Where(t => t.SortingID == SortingID&& t.Status == (int)Enums.Status.Normal).FirstOrDefault().PackingID;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { OrderID = CurrentOrder.ID }, t => t.ID == packingID);
                    }

                    // 5、无通知产品录入
                    //插入新的OrderItem 以及装箱信息
                    //新插入的OrderItem 是 无通知产品录入，需要插入EntryNoticeItem、Sorting、Packing、PackingItem 信息
                    var newOrderItems = SelectedItems.Where(t => string.IsNullOrEmpty(t.OrderItemID)).ToList();
                    foreach (var orderItem in newOrderItems)
                    {
                        var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                        OrderItem newOrderItem = new OrderItem();
                        string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                        orderItem.NewOrderItemID = singleOrderItemID;

                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItems
                        {
                            ID = singleOrderItemID,
                            OrderID = CurrentOrder.ID,
                            Name = orderItem.Name,
                            Model = orderItem.Model,
                            Manufacturer = orderItem.Brand,
                            Batch = orderItem.BatchNo,
                            Origin = orderItem.Origin,
                            Quantity = orderItem.Qty,
                            Unit = orderItem.Unit,
                            UnitPrice = orderItem.UnitPrice,
                            TotalPrice = orderItem.TotalPrice,
                            IsSampllingCheck = false,
                            ClassifyStatus = (int)Enums.ClassifyStatus.Unclassified,
                            Status = (int)Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });

                        var UnExcepted = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>().Where(t => t.ID == orderItem.ID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>(new { IsMapped = true }, item => item.ID == UnExcepted.ID);
                        //判断是否已经生成了装箱信息
                        var packingInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>().Where(t => t.OrderID == CurrentOrder.ID && t.BoxIndex == UnExcepted.BoxIndex).FirstOrDefault();
                        string newPackingID = "";
                        if (packingInfo != null)
                        {
                            newPackingID = packingInfo.ID;
                        }
                        else
                        {
                            newPackingID = Needs.Overall.PKeySigner.Pick(PKeyType.Packing);
                            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Packings
                            {
                                ID = newPackingID,
                                OrderID = CurrentOrder.ID,
                                AdminID = "",
                                BoxIndex = UnExcepted.BoxIndex,
                                PackingDate = UnExcepted.CreateDate,
                                Weight = UnExcepted.GrossWeight,
                                WrapType = "22",
                                PackingStatus = (int)Enums.PackingStatus.UnSealed,
                                Status = (int)Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                            });
                        }

                        string EntryNoticeItemID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EntryNoticeItems
                        {
                            ID = EntryNoticeItemID,
                            EntryNoticeID = EntryNoticeID,
                            OrderItemID = singleOrderItemID,
                            EntryNoticeStatus = (int)Enums.EntryNoticeStatus.Boxed,
                            Status = (int)Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });

                        string SortingID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Sortings
                        {
                            ID = SortingID,
                            OrderID = CurrentOrder.ID,
                            OrderItemID = singleOrderItemID,
                            AdminID = "",
                            EntryNoticeItemID = EntryNoticeItemID,
                            WarehouseType = (int)Enums.WarehouseType.HongKong,
                            Quantity = UnExcepted.Qty,
                            BoxIndex = UnExcepted.BoxIndex,
                            NetWeight = UnExcepted.GrossWeight * 0.7M,
                            GrossWeight = UnExcepted.GrossWeight,
                            DecStatus = (int)Enums.SortingDecStatus.No,
                            Status = (int)Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });

                        string packingItemID = Needs.Overall.PKeySigner.Pick(PKeyType.PackingItem);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PackingItems
                        {
                            ID = packingItemID,
                            PackingID = newPackingID,
                            SortingID = SortingID,
                            Status = (int)Enums.Status.Normal,
                            CreateDate = DateTime.Now
                        });
                    }

                    reponsitory.Submit();

                    //6、改国际运单信息
                    // 根据OrderItemID 查Sorting，根据SortingID 查OrderWaybillItems，再改OrderWaybills
                    var OrderWaybillIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.OrderItemID)).Select(t => t.OrderItemID).ToList();
                    foreach(var orderItemID in OrderWaybillIDs)
                    {
                        var sorting = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().Where(t => t.OrderItemID == orderItemID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
                        if (sorting != null)
                        {
                            var OrderWaybillItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>().Where(t => t.SortingID == sorting.ID).FirstOrDefault();
                            if (OrderWaybillItems != null)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWaybills>(new {OrderID = CurrentOrder.ID }, t => t.ID == OrderWaybillItems.OrderWaybillID);
                            }
                        }
                    }

                    //改原订单的报关价格
                    var declarePrice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == OriginOrder.ID).Sum(t => t.TotalPrice);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { DeclarePrice = declarePrice }, item => item.ID == OriginOrder.ID);

                    reponsitory.Submit();
                }
            }
            catch(Exception ex)
            {
                ex.CcsLog("订单ID【" + CurrentOrder.ID + "】拆分订单异常!");
            }
           
  
        }

        public List<MatchViewModel> GetMatchViewModels()
        {
            return SelectedItems;
        }
    }
}
