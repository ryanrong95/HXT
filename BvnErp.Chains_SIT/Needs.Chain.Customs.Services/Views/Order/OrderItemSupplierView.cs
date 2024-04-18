using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 按照供应商查型号信息
    /// </summary>
    public class OrderItemSupplierView
    {
        private ScCustomsReponsitory _reponsitory { get; set; }

        public OrderItemSupplierView()
        {
            this._reponsitory = new ScCustomsReponsitory();
        }

        public OrderItemSupplierView(ScCustomsReponsitory reponsitory)
        {
            this._reponsitory = reponsitory;
        }

        public List<OrderItemSupplierViewModel> GetInfos(string orderID)
        {
            var orderItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var productSupplierMapView = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductSupplierMap>();
            var clientSuppliers = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();

            var linq = from orderItem in orderItems
                       join productSupplierMap in productSupplierMapView on orderItem.ProductUniqueCode equals productSupplierMap.ID
                       join clientSupplier in clientSuppliers on productSupplierMap.SupplierID equals clientSupplier.ID
                       where orderItem.OrderID == orderID
                          && clientSupplier.Status == (int)Enums.Status.Normal
                       select new OrderItemSupplierViewModel
                       {
                           SupplierID = productSupplierMap.SupplierID,
                           SupplierName = clientSupplier.Name,
                           OrderItemID = orderItem.ID,
                           Model = orderItem.Model,
                           Quantity = orderItem.Quantity,
                           UnitPrice = orderItem.UnitPrice,
                           TotalPrice = orderItem.TotalPrice,
                       };

            return linq.ToList();
        }

    }

    public class OrderItemSupplierViewModel
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 型号ID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

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
    }

}
