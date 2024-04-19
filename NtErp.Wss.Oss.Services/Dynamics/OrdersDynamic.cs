using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Dynamics
{


    public class OrdersDynamic : QueryView9<OrdersDynamic.Ouputer, CvOssReponsitory>
    {
        public class Ouputer
        {
            public string ID { get; set; }
            public decimal? Paid { get; set; }
            public decimal? Total { get; set; }
            public dynamic Items { get; set; }
            public dynamic Port { get; set; }
        }

        public OrdersDynamic()
        {

        }

        internal OrdersDynamic(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal OrdersDynamic(CvOssReponsitory reponsitory, IQueryable<Ouputer> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public OrdersDynamic SearchByProductName(string name)
        {
            var linq = from prodcut in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.StandardProducts>()
                       join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.OrderItems>() on prodcut.ID equals item.ProductID
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Orders>() on item.OrderID equals order.ID
                       where prodcut.Name.StartsWith(name)
                       select order;

            return new OrdersDynamic(this.Reponsitory, this.GetIQueryable(linq));
        }

        IQueryable<Ouputer> GetIQueryable(IQueryable<Layer.Data.Sqls.CvOss.Orders> ordersView)
        {
            var linq_items = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.OrderItems>()
                             join product in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.StandardProducts>() on item.ProductID equals product.ID
                             select new
                             {
                                 item.ID,
                                 item.OrderID,
                                 productName = product.Name,
                             };

            return from order in ordersView
                   join item in linq_items on order.ID equals item.OrderID into items
                   select new Ouputer
                   {
                       ID = order.ID,
                       Paid = order.Paid,
                       Total = order.Total,
                       Items = items.Select(orderItem => new
                       {
                           orderItem.ID,
                           orderItem.productName,
                       }),
                       Port = new
                       {
                           orderid = order.TransportTerms.ID,
                           order.TransportTerms.Carrier
                       },
                   };
        }

        protected override IQueryable<Ouputer> GetIQueryable()
        {
            return this.GetIQueryable(this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Orders>());
        }
    }
}
