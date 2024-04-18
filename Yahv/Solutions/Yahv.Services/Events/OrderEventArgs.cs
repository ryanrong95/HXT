using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Events
{
    public delegate void OrderHandler(object sender, OrderEventArgs e);

    public class OrderEventArgs : EventArgs
    {
        public string OrderID { get; private set; }

        public Currency Currency { get; set; }

        public DateTime? OrderCreateDate { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        public OrderEventArgs(string orderId)
        {
            this.OrderID = orderId;
        }
    }
}
