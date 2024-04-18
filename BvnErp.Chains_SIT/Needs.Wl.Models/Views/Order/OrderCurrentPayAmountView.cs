using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 订单本次申请金额视图
    /// </summary>
    public class OrderCurrentPayAmountView : View<Models.OrderCurrentPayAmount, ScCustomsReponsitory>
    {
        private string OrderID { get; set; }

        private string SupplierID { get; set; }

        public OrderCurrentPayAmountView(string orderID, string supplierID)
        {
            this.OrderID = orderID;
            this.SupplierID = supplierID;
            this.AllowPaging = false;
        }

        protected override IQueryable<OrderCurrentPayAmount> GetIQueryable()
        {
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var productSupplierMaps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductSupplierMap>();

            return from orderItem in orderItems
                   join productSupplierMap in productSupplierMaps on orderItem.ProductUniqueCode equals productSupplierMap.ID
                   where orderItem.OrderID == this.OrderID
                       && productSupplierMap.SupplierID == this.SupplierID
                       && orderItem.Status == (int)Enums.Status.Normal
                   group orderItem by new { orderItem.OrderID, } into g
                   select new OrderCurrentPayAmount
                   {
                       ID = g.Key.OrderID,
                       IsMatchSupplier = true,
                       CurrentPaidAmount = g.Sum(t => t.TotalPrice),
                   };
        }
    }
}
