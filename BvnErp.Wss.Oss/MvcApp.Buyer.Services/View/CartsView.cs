using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.View
{
    public class CartsView : QueryView<Models.Cart, BvOrdersReponsitory>
    {
        public CartsView()
        {

        }
        internal CartsView(BvOrdersReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Cart> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.Carts>()
                       select new Models.Cart
                       {
                           ServiceOutputID = entity.ServiceOutputID,
                           ServiceInputID = entity.ServiceInputID,
                           UserID = entity.UserID,
                           CustomerCode = entity.CustomerCode,
                           ProductSign = entity.ProductSign,
                           Quantity = entity.Quantity,
                           Xml = entity.Xml,
                       };

            return linq;
        }

    }

}
