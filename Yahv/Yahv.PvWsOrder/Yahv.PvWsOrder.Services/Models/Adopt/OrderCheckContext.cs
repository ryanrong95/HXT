using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models.Adopt
{
    public class OrderCheckContext
    {
        public IOrderCheck OrderCheck { get; set; }

        public OrderCheckContext(IOrderCheck orderCheck)
        {
            this.OrderCheck = orderCheck;
        }

        public bool Check(string orderID)
        {
            return OrderCheck.isVaildOrder(orderID);
        }

        public void UpdateFee(string orderID,decimal orderFee,string adminID)
        {
            OrderCheck.UpdateFee(orderID, orderFee, adminID);
        }
    }
}
