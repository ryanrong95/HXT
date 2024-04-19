using NtErp.Wss.Sales.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders.Delivery
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {

        public OrderMain Order
        {
            get
            {
                string id = Request["id"];
                return new NtErp.Wss.Sales.Services.Views.OrderMainsView().SingleOrDefault(t => t.ID == id);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void save()
        {
            float ratio = float.Parse(Request["_ratio"]);

            var model = this.Order;
            model.DeliveryRatio = ratio;
            model.Enter();
        }
    }
}