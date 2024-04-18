using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.OrderChange
{
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string orderID = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(orderID))
            {
                var orderItemChange = new Needs.Ccs.Services.Views.OrderItemChangeView().Where(x=>x.OrderID==orderID);
                Func<OrderItemChangeNotice, object> convert = item => new
                {
                    ProductModel = item.ProductModel,
                    Type = item.Type.GetDescription(),
                    item.CreateDate,
                    Adder = item.Sorter.RealName,
                };
                this.Paging(orderItemChange, convert);
                //Response.Write(new
                //{
                //    rows = orderItemChange.Select(convert).ToArray(),
                //    total = orderItemChange.Count()
                //}.Json());
            }
        }

    }
}