using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public abstract class BaseStrategy
    {
        public  List<InsideOrderItem> models { get; set; }

        public BaseStrategy(List<InsideOrderItem> Models)
        {
            this.models = Models;
        }
        public abstract List<OrderModel> SplitOrder();
    }
}
