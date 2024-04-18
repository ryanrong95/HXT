using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderItemUnitPriceLogsView : UniqueView<Models.Log_OrderItemPrice, ScCustomsReponsitory>
    {
        public OrderItemUnitPriceLogsView()
        {
        }

        public OrderItemUnitPriceLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Log_OrderItemPrice> GetIQueryable()
        {
            return from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderItem.OrderID equals order.ID
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   select new Models.Log_OrderItemPrice
                   {
                       ID = orderItem.ID,
                       Model = orderItem.Model,
                       UnitPrice = orderItem.UnitPrice,
                       Currency = order.Currency,
                       Quantity = orderItem.Quantity,
                       CreateDate = orderItem.CreateDate,
                       ClientName = company.Name,
                       OrderID = order.ID,
                       OrderStatus = (Enums.OrderStatus)order.OrderStatus
                   };
        }
    }
}
