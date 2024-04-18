using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeliveryOrderItem: IUnique
    {
        public string ID { get; set; }
        public string DeliveryOrderID { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Qty { get; set; }
    }
}
