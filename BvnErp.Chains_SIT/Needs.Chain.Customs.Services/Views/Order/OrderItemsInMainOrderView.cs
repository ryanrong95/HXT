using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderItemsInMainOrderView : UniqueView<CurrentOrderInfo, ScCustomsReponsitory>
    {
        protected override IQueryable<CurrentOrderInfo> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        public List<Models.OrderItem> GetOrderItemsByMainOrderID(string mainOrderID)
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var linq = from orderItem in orderItems
                       join order in orders
                            on new
                            {
                                OrderID = orderItem.OrderID,
                                OrderItemDataStatus = orderItem.Status,
                                OrderDataStatus = (int)Enums.Status.Normal,
                                MainOrderId = mainOrderID,
                            }
                            equals new
                            {
                                OrderID = order.ID,
                                OrderItemDataStatus = (int)Enums.Status.Normal,
                                OrderDataStatus = order.Status,
                                MainOrderId = order.MainOrderId,
                            }
                       select new Models.OrderItem
                       {
                           ID = orderItem.ID,
                           OrderID = orderItem.OrderID,
                           MainOrderID = order.MainOrderId,
                           Origin = orderItem.Origin,
                           Quantity = orderItem.Quantity,
                           Unit = orderItem.Unit,
                           UnitPrice = orderItem.UnitPrice,
                           TotalPrice = orderItem.TotalPrice,
                           Model = orderItem.Model,
                           Manufacturer = orderItem.Manufacturer,
                           Batch = orderItem.Batch,
                       };

            return linq.ToList();
        }

        /// <summary>
        /// 根据大订单ID 查询其中的 OrderItem 信息
        /// 未查 InputID，GrossWeight
        /// </summary>
        /// <param name="MainOrderID"></param>
        /// <returns></returns>
        public CurrentOrderInfo GetCurrentOrderInfo(string[] orderItemIDs)
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var linq = from orderItem in orderItems
                       where orderItemIDs.Contains(orderItem.ID)
                       select new OrderItemChanges
                       {
                           OrderItemID = orderItem.ID,
                           Product = new CenterProduct()
                           {
                               PartNumber = orderItem.Model,
                               Manufacturer = orderItem.Manufacturer,
                           },
                           Origin = orderItem.Origin,
                           DateCode = orderItem.Batch,
                           Quantity = orderItem.Quantity,
                           UnitPrice = orderItem.UnitPrice,
                           Unit = orderItem.Unit,
                           TinyOrderID = orderItem.OrderID,
                       };

            List<OrderItemChanges> orderItemChanges = linq.ToList();

            Models.Order oneOrder = (from order in orders
                                     where order.ID == orderItemChanges[0].TinyOrderID
                                     select new Models.Order
                                     {
                                         MainOrderID = order.MainOrderId,
                                         Currency = order.Currency,
                                     }).FirstOrDefault();

            CurrentOrderInfo currentOrderInfo = new CurrentOrderInfo()
            {
                OrderID = oneOrder.MainOrderID,
                Currency = oneOrder.Currency,
                Confirmed = false,
                items = orderItemChanges,
            };

            return currentOrderInfo;
        }

        public CurrentOrderInfo GetInfoByMainOrderID(string mainOrderID)
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            return (from order in orders
                    where order.MainOrderId == mainOrderID && order.Status == (int)Enums.Status.Normal
                    select new CurrentOrderInfo
                    {
                        OrderID = order.MainOrderId,
                        Currency = order.Currency,
                    }).FirstOrDefault();
        }
    }

    public class CurrentOrderInfo : IUnique
    {
        public string ID { get; set; } = string.Empty;

        public string OrderID { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 订单是否重新确认
        /// </summary>
        public bool Confirmed { get; set; }

        public List<OrderItemChanges> items { get; set; }
        public List<string> OriginOrderItemIDs { get; set; }
    }

    public class OrderItemChanges
    {
        public string OrderItemID { get; set; } = string.Empty;

        public string InputID { get; set; } = string.Empty;

        public string CustomName { get; set; } = string.Empty;

        public CenterProduct Product { get; set; }

        public string Origin { get; set; } = string.Empty;

        public string DateCode { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        //public decimal GrossWeight { get; set; }

        public string Unit { get; set; } = string.Empty;

        public string TinyOrderID { get; set; } = string.Empty;
        public string OriginOrderItemID { get; set; }
    }

    public class CenterProduct
    {
        //public string ID { get; set; }

        public string PartNumber { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;

        //public string PackageCase { get; set; }

        //public string Packaging { get; set; }

        //public DateTime CreateDate { get; set; }
    }


}
