using NtErp.Wss.Oss.Services.Extends;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Premiums
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = Request.Form["_name"]; // 名称
            string summary = Request.Form["_summary"]; // 备注
            int count = int.Parse(Request["_count"]); // 数量
            decimal price = decimal.Parse(Request.Form["_price"]); // 金额

            new Premium
            {
                OrderID = Request["orderid"],
                OrderItemID = Request["itemid"],
                Name = name,
                Count = count,
                Price = price,
                Summary = summary,
            }.Enter();

            Alert(this.hSuccess.Value, Request.Url, true);
        }
    }
}