using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 订单本次申请金额视图
    /// </summary>
    public class OrderCurrentPayAmountView : UniqueView<OrderCurrentPayAmount, ScCustomReponsitory>
    {
        private string OrderID { get; set; }

        private string SupplierID { get; set; }

        public OrderCurrentPayAmountView(string orderID, string supplierID)
        {
            this.OrderID = orderID;
            this.SupplierID = supplierID;
        }

        protected override IQueryable<OrderCurrentPayAmount> GetIQueryable()
        {
            var orderItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItems>();
            var productSupplierMaps = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ProductSupplierMap>();

            return from orderItem in orderItems
                   join productSupplierMap in productSupplierMaps on orderItem.ProductUniqueCode equals productSupplierMap.ID
                   where orderItem.OrderID == this.OrderID
                       && productSupplierMap.SupplierID == this.SupplierID
                       && orderItem.Status == (int)Status.Normal
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
