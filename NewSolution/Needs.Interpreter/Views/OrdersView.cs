using Layer.Data.Sqls;
using Needs.Interpreter.Model;
using Needs.Linq;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Domain
{
    public class OrdersView : UniqueView<Order, BvTesterReponsitory>, ISearch<OrdersView>
    {
        public OrdersView()
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvTester.Orders>()
                   select new Order
                   {
                       ID = order.ID,
                       Name = order.Name,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                   };
        }

        public OrdersView Search(string name, string value)
        {
            //var query = from linq in this.GetIQueryable()
            //            join search in this.Reponsitory.GetTable<DataLayer.Linq.Erps.AdminSearchers>() on linq.ID equals search.MainID
            //            where search.Name == name && search.Value == value
            //            select linq;

            //return new AdminsView(this.Reponsitory, query);

            return null;
        }
    }
}
