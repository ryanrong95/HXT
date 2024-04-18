using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    public delegate void OrderDeliveryConfirmedEventHanlder(object sender, OrderDeliveryConfirmedEventArgs e);
    public class OrderDeliveryConfirmedEventArgs : EventArgs
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }
       
        public OrderDeliveryConfirmedEventArgs(List<Models.OrderItemAssitant> orderItems)
        {
            this.OrderItems = orderItems;           
        }

        public OrderDeliveryConfirmedEventArgs() { }
    }

}
