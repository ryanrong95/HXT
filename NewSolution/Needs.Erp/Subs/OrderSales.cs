using Needs.Erp.Generic;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Erp.Models
{
    /// <summary>
    /// Admin 
    /// </summary>
    public partial class Admin
    {
        public OrderSales OrderSales { get { return new OrderSales(this); } }
    }
}

namespace Needs.Erp
{
    public class OrderSales
    {
        IGenericAdmin Admin;

        public OrderSales(IGenericAdmin admin)
        {
            this.Admin = admin;
        }
        /// <summary>
        /// 客户
        /// </summary>
        public ClientsTopView Clients
        {
            get { return new ClientsTopView(); }
        }
        /// <summary>
        /// 我的订单
        /// </summary>
        public MyOrdersView MyOrders
        {
            get
            {
                return new MyOrdersView(this.Admin);
            }
        }

    }

}
