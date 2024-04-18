using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class ItemStrategy:BaseStrategy
    {
        public ItemStrategy(List<InsideOrderItem> Models) : base(Models)
        {

        }

        public override List<OrderModel> SplitOrder()
        {
            List<OrderModel> orderOne = new List<OrderModel>();
            List<PackingModel> wouldBeOrders = new List<PackingModel>();

            if (base.models.Count() <= 50)
            {
                List<string> ProductUniqueCodes = new List<string>();
                foreach (var t in base.models)
                {
                    ProductUniqueCodes.Add(t.PreProductID);
                }
                OrderModel om = new OrderModel();
                om.ProductUniqueCodes = ProductUniqueCodes;
                orderOne.Add(om);
            }
            else
            {
                var limit = 50;
                for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(base.models.Count() / 50M)); i++)
                {
                    var mc = base.models.Skip(i * limit).Take(limit).ToList();
                    List<string> ProductUniqueCodes = new List<string>();
                    foreach (var t in mc)
                    {
                        ProductUniqueCodes.Add(t.PreProductID);
                    }
                    OrderModel om = new OrderModel();
                    om.ProductUniqueCodes = ProductUniqueCodes;
                    orderOne.Add(om);
                }
            }

            return orderOne;
        }
    }
}
