using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public  class Log_OrderItemPrice:IUnique
    {
        public string ID { get; set; }
        public string Model { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string OrderID { get; set; }
        public Enums.OrderStatus OrderStatus { get; set; }
    }
}
