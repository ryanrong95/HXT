using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models

{
    public class SCPersistentHandlerCon : SCHandler
    {
        public SCPersistentHandlerCon(List<MatchViewModel> selectedItems, Order currentOrder)
        {
            SelectedItems = selectedItems;          
            CurrentOrder = currentOrder;
        }

        /// <summary>
        /// 确认订单，持久化要做的事情：
        /// 1、无通知产品录入
        ///    - 新增OrderItems 中的项,
        ///    - 根据oldOrderID 和Model 查询UnExpectedOrderItem
        ///      -> 用箱号和newOrderID 去查询Packing，没有就新增，有就取PackingID
        ///    - 新增EntryNoticeItems,EntryNoticeID为新增的EntryNoticeID  
        ///    - 新增Sorting OrderID:新增的OrderID,OrderItemID:新增的OrderItemID,EntryNoticeItemID:新增的EntryNoticeItemID
        ///                  BoxIndex:根据Model 在去UnExpectedOrderItem找，NetWeight、GrossWeight 不需要自动分配
        ///    - 新增PackingItem，SortingID为新增的SortingID, PackingID 为新增的PackingID           
        ///                  
        /// 2、更改原 Order 中的报关价格
        /// </summary>
        public override void handleRequest()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    //插入新的OrderItem
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

                        var UnExcepted = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UnExpectedOrderItem>().Where(t => t.OrderID == CurrentOrder.ID && t.Model == orderItem.Model && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
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

                        string EntryNoticeID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Where(t => t.OrderID == CurrentOrder.ID && t.Status == (int)Enums.Status.Normal).FirstOrDefault().ID;
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

                    //改原订单的报关价格
                    var declarePrice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == CurrentOrder.ID).Sum(t => t.TotalPrice);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { DeclarePrice = declarePrice }, item => item.ID == CurrentOrder.ID);

                    reponsitory.Submit();
                }

                if (next != null)
                {
                    next.handleRequest();
                }
            }
            catch(Exception ex)
            {
                ex.CcsLog("订单ID【" + CurrentOrder.ID + "】确认订单异常!");
            }
           
        }

        public List<MatchViewModel> GetMatchViewModels()
        {
            return SelectedItems;
        }
    }
}
