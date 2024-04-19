using Needs.Linq;
using NtErp.Wss.Sales.Services.Models.Carts;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class CartsView : QueryView<Cart, Layer.Data.Sqls.BvOrdersReponsitory>
    {
        public CartsView()
        {
        }
        
        protected override IQueryable<Cart> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.Carts>()
                        select new Cart(entity.Xml)
                        {
                            ServiceOutputID = entity.ServiceOutputID,
                            ServiceInputID = entity.ServiceInputID,
                            CustomerCode = entity.CustomerCode,
                            Quantity = entity.Quantity,
                            ProductSign = entity.ProductSign,
                            UserID = entity.UserID,
                            CreateDate = entity.CreateDate,
                            UpdateDate = entity.UpdateDate,
                        };

            return linqs;
        }


    }
}
