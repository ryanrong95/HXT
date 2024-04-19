using Needs.Linq;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class OrdersView : QueryView<Order, Layer.Data.Sqls.BvOrdersReponsitory>
    {
        public OrdersView()
        {
        }



        protected override IQueryable<Order> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.Orders>()
                        join xml in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.OrderShowers>() on entity.ID equals xml.MainID
                        select new Order(xml.Xml)
                        {
                            ID = entity.ID,
                            UserID = entity.UserID,
                            SiteUserName = entity.SiteUserName,
                            CompanyName = entity.CompanyName,
                            Currency = (Underly.Currency)entity.Currency,
                            District = (Underly.District)entity.District,
                            Transport = (TransportTerm)entity.Transport,
                            CreateDate = entity.CreateDate,
                            UpdateDate = entity.UpdateDate,
                            Status = (OrderStatus)entity.Status,
                            DeliveryRatio = entity.DeliveryRatio.HasValue ? (float)entity.DeliveryRatio : 0,
                            PaidRatio = entity.PaidRatio.HasValue ? (float)entity.PaidRatio : 0,
                            Summary = entity.Summary
                        };

            return linqs;
        }


    }
}
