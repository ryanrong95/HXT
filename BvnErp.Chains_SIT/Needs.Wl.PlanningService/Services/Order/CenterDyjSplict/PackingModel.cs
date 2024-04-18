using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class dyjPackingModel
    {
        public int AllCount { get; set; }

        public List<string> boxes { get; set; }

        public dyjPackingModel()
        {
            boxes = new List<string>();
        }
    }

    public class OrderModel
    {
        public List<string> ProductUniqueCodes { get; set; }

        public OrderModel()
        {
            ProductUniqueCodes = new List<string>();
        }
    }
}
