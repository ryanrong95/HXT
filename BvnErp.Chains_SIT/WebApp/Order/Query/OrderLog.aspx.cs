using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Query
{
    public partial class OrderLog : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[ID];
            var logs = order.Logs.OrderByDescending(item => item.CreateDate);
            Func<Needs.Ccs.Services.Models.OrderLog, object> convert = log => new
            {
                //People = log.Admin != null ? log.Admin.RealName : log.User.RealName,
                //Orderstatus = log.OrderStatus.GetDescription(),
                CreateDate = log.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = log.Summary,
            };
            Response.Write(new
            {
                rows = logs.Select(convert).ToList(),
                total = logs.Count()
            }.Json());
        }
    }
}