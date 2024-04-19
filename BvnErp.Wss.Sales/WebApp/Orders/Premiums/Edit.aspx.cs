using NtErp.Wss.Sales.Services;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Utils.Structures;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders.Premiums
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected Order Order
        {
            get
            {
                var id = Request["id"];
                return new OrdersView().SingleOrDefault(item => item.ID == id && (item.Status != NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus.Closed || item.Status != NtErp.Wss.Sales.Services.Underly.Orders.OrderStatus.Completed)) as Order;
            }
        }
        protected string Currency;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.Order == null)
                {
                    Response.Write("订单不存在！");
                    Response.End();
                }
                this.Currency = Order.Currency.GetTitle();
            }
        }

        protected void save()
        {
            string name = Request.Form["_name"], // 名称
               summary = Request.Form["_summary"]; // 备注
            decimal price = decimal.Parse(Request.Form["_price"]); // 金额
            if (price == 0)
            {
                Response.Write(new
                {
                    success = false,
                    code = -1 // 金额为0
                }.Json());
                return;
            }
            try
            {
                var model = new NtErp.Wss.Sales.Services.Model.Orders.Premium
                {
                    Name = name,
                    Price = price,
                    Summary = summary,
                    Quantity = 1
                };
                this.Order.AddPremium(model);

                Response.Write(new
                {
                    success = true,
                    code = 200 // 金额为0
                }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new
                {
                    success = false,
                    code = -2 // 金额为0
                }.Json());
            }


        }

        protected void Save_Click(object sender, EventArgs e)
        {

            string name = Request.Form["_name"], // 名称
                summary = Request.Form["_summary"]; // 备注
            decimal price = decimal.Parse(Request.Form["_price"]); // 金额

            if (price == 0)
            {
                return;
            }
            var model = new NtErp.Wss.Sales.Services.Model.Orders.Premium
            {
                Name = name,
                Price = price,
                Summary = summary,
                Quantity = 1
            };

            this.Order.AddPremium(model);

            Response.Write("添加成功！");
            Response.End();
        }
    }
}