using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 订单详情中使用 OrderItems 列表视图
    /// </summary>
    public class OrderDetailOrderItemsView : View<OrderDetailOrderItemsModel, ScCustomsReponsitory>
    {
        private string OrderID { get; set; } = string.Empty;

        private Enums.OrderType OrderType { get; set; }

        public OrderDetailOrderItemsView(string orderID, Enums.OrderType orderType)
        {
            this.OrderID = orderID;
            this.OrderType = orderType;
        }

        protected override IQueryable<OrderDetailOrderItemsModel> GetIQueryable()
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                .Where(t => t.OrderID == this.OrderID
                         && t.Status == (int)Enums.Status.Normal);
            var orderItemCategories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

            var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();

            var orderitemsLinqs = from orderItem in orderItems
                                  join orderItemCategory in orderItemCategories
                                       on new
                                       {
                                           OrderItemID = orderItem.ID,
                                           OrderItemDataStatus = orderItem.Status,
                                           OrderItemCategoryDataStatus = (int)Enums.Status.Normal,
                                       }
                                       equals new
                                       {
                                           OrderItemID = orderItemCategory.OrderItemID,
                                           OrderItemDataStatus = (int)Enums.Status.Normal,
                                           OrderItemCategoryDataStatus = orderItemCategory.Status,
                                       }
                                       into orderItemCategories2
                                  from orderItemCategory in orderItemCategories2.DefaultIfEmpty()
                                  orderby orderItem.ID
                                  select new OrderDetailOrderItemsModel
                                  {
                                      OrderItemID = orderItem.ID,
                                      OrderItemName = orderItem.Name,
                                      OrderItemCategoryName = orderItemCategory.Name,
                                      Manufacturer = orderItem.Manufacturer,
                                      Model = orderItem.Model,
                                      Quantity = orderItem.Quantity,
                                      UnitPrice = orderItem.UnitPrice,
                                      TotalPrice = orderItem.TotalPrice,
                                      Unit = orderItem.Unit,
                                      Origin = orderItem.Origin,
                                      GrossWeight = orderItem.GrossWeight,
                                      HSCode = orderItemCategory.HSCode,
                                      CategoryType = orderItemCategory == null ? Enums.ItemCategoryType.Normal : (Enums.ItemCategoryType)orderItemCategory.Type,
                                  };

            if (this.OrderType == Enums.OrderType.Outside)
            {
                var orderitemIDs = orderitemsLinqs.Select(t => t.OrderItemID);

                var sortingsLinqs = from sorting in sortings
                                    where sorting.Status == (int)Enums.Status.Normal
                                       && sorting.WarehouseType == (int)Enums.WarehouseType.HongKong
                                       && orderitemIDs.Contains(sorting.OrderItemID)
                                    group sorting by new { sorting.OrderItemID } into g
                                    select new OrderDetailOrderItemsModel
                                    {
                                        OrderItemID = g.Key.OrderItemID,
                                        IsHkPacked = (g.First() != null),
                                    };

                var results = from orderitemsLinq in orderitemsLinqs
                              join sortingsLinq in sortingsLinqs on orderitemsLinq.OrderItemID equals sortingsLinq.OrderItemID into sortingsLinqs2
                              from sortingsLinq in sortingsLinqs2.DefaultIfEmpty()
                              select new OrderDetailOrderItemsModel
                              {
                                  OrderItemID = orderitemsLinq.OrderItemID,
                                  OrderItemName = orderitemsLinq.OrderItemName,
                                  OrderItemCategoryName = orderitemsLinq.OrderItemCategoryName,
                                  Manufacturer = orderitemsLinq.Manufacturer,
                                  Model = orderitemsLinq.Model,
                                  Quantity = orderitemsLinq.Quantity,
                                  UnitPrice = orderitemsLinq.UnitPrice,
                                  TotalPrice = orderitemsLinq.TotalPrice,
                                  Unit = orderitemsLinq.Unit,
                                  Origin = orderitemsLinq.Origin,
                                  GrossWeight = orderitemsLinq.GrossWeight,
                                  IsHkPacked = (sortingsLinq != null),
                                  CategoryType = orderitemsLinq.CategoryType,
                              };

                return results;
            }
            else
            {
                return orderitemsLinqs;
            }

        }

        /// <summary>
        /// 获取对应报关单是否报关成功 IsSuccess 状态
        /// </summary>
        /// <returns></returns>
        public bool GetDecHeadIsSuccess()
        {
            var decHead = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                .Where(t => t.OrderID == this.OrderID).FirstOrDefault();
            if (decHead == null)
            {
                return false;
            }

            return decHead.IsSuccess;
        }

        public Enums.EntryNoticeStatus GetHkEntryNoticeStatus()
        {
            Enums.EntryNoticeStatus hkEntryNoticeStatus = Enums.EntryNoticeStatus.UnBoxed;

            var hkEntryNotice = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>()
                .Where(t => t.OrderID == this.OrderID
                         && t.WarehouseType == (int)Enums.WarehouseType.HongKong
                         && t.Status == (int)Enums.Status.Normal).FirstOrDefault();
            if (hkEntryNotice != null)
            {
                hkEntryNoticeStatus = (Enums.EntryNoticeStatus)hkEntryNotice.EntryNoticeStatus;
            }

            return hkEntryNoticeStatus;
        }
    }

    public class OrderDetailOrderItemsModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string OrderItemID { get; set; } = string.Empty;

        /// <summary>
        /// OrderItem 品名
        /// </summary>
        public string OrderItemName { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemCategory 品名
        /// </summary>
        public string OrderItemCategoryName { get; set; } = string.Empty;

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 该型号香港是否装箱
        /// </summary>
        public bool IsHkPacked { get; set; } = false;

        /// <summary>
        /// 是否显示修改按钮
        /// </summary>
        public bool IsShowModifyBtn { get; set; } = true;

        /// <summary>
        /// 不显示修改按钮原因
        /// </summary>
        public string NotShowReason { get; set; } = string.Empty;

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; } = string.Empty;
        /// <summary>
        /// 归类类型
        /// </summary>
        public Enums.ItemCategoryType CategoryType { get; set; }
        public string CategoryTypeName { get; set; }
    }
}
