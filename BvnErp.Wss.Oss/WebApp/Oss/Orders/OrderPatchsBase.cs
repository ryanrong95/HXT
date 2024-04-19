using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Oss.Orders
{
    public class OrderPatchsBase : Needs.Web.Sso.Forms.ErpPage
    {
        /// <summary>
        /// 订单
        /// </summary>
        NtErp.Wss.Oss.Services.Models.Order Order
        {
            get
            {
                var oid = Request["orderid"];
                return Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[oid];
            }
        }

        public OrderPatchsBase()
        {
            this.Load += OrderPatchsBase_Load;
        }

        private void OrderPatchsBase_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = this.Order;
            }
        }
    }
}