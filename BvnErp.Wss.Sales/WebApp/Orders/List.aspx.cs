using Needs.Web;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Orders
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void data()
        {
            var orderid = Request["orderid"];
            var user = Request["user"];
            OrderStatus status;
            District district;
            Currency currency;
            TransportTerm transport;
            DateTime starttime, endtime;
            int pratio = -1;
            int dratio = -1;

            var views = new NtErp.Wss.Sales.Services.Views.OrderMainsView().AsQueryable();
            if (!string.IsNullOrWhiteSpace(orderid))
            {
                views = views.Where(t => t.ID.Contains(orderid));
            }
            if (!string.IsNullOrWhiteSpace(user))
            {
                var userids = new NtErp.Wss.Sales.Services.Views.ClientTopAlls().Where(t => t.UserName.Contains(user) || t.ID.Contains(user)).Select(t => t.ID).ToArray();
                views = views.Where(t => userids.Contains(t.UserID));
            }
            if (Enum.TryParse(Request["status"], out status) && (int)status != -1)
            {
                views = views.Where(t => t.Status == status);
            }
            if (Enum.TryParse(Request["district"], out district) && (int)district != -1)
            {
                views = views.Where(t => t.District == district);
            }
            if (Enum.TryParse(Request["currency"], out currency) && (int)currency != -1)
            {
                views = views.Where(t => t.Currency == currency);
            }
            if (Enum.TryParse(Request["transport"], out transport) && (int)transport != -1)
            {
                views = views.Where(t => t.Transport == transport);
            }
            if (int.TryParse(Request["pratio"], out pratio) && pratio != -1)
            {
                if (pratio == 1)
                {
                    views = views.Where(t => t.PaidRatio == 0);
                }
                else if (pratio == 3)
                {
                    views = views.Where(t => t.PaidRatio >= 100);
                }
                else
                {
                    views = views.Where(t => t.PaidRatio < 100 && t.PaidRatio > 0);
                }
            }
            if (int.TryParse(Request["dratio"], out dratio) && dratio != -1)
            {
                if (dratio == 1)
                {
                    views = views.Where(t => t.DeliveryRatio == 0);
                }
                else if (dratio == 3)
                {
                    views = views.Where(t => t.DeliveryRatio >= 100);
                }
                else
                {
                    views = views.Where(t => t.DeliveryRatio < 100 && t.DeliveryRatio > 0);
                }
            }
            if (DateTime.TryParse(Request["starttime"], out starttime))
            {
                views = views.Where(t => t.CreateDate >= starttime);
            }
            if (DateTime.TryParse(Request["endtime"], out endtime))
            {
                views = views.Where(t => t.CreateDate < endtime.AddDays(1));
            }


            Response.Paging(views.OrderByDescending(t => t.CreateDate), item => new
            {
                ID = item.ID,
                SiteUserName = item.SiteUserName,
                Currency = item.Currency.ToString() + string.Concat("(", item.Currency.GetTitle(), ")"),
                District = item.District.GetTitle(),
                StatusStr = item.Status.GetTitle(),
                Transport = item.Transport.GetTitle(),
                UserID = item.UserID,
                Summary = item.Summary,
                DeliveryRatio = item.DeliveryRatio + "%",
                PaidRatio = item.PaidRatio + "%",
                IsSend = item.DeliveryRatio > 0 ? true : false,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate
            });

        }

        protected NtErp.Wss.Sales.Services.Order Order
        {
            get
            {
                var id = Request["id"];
                return new NtErp.Wss.Sales.Services.Views.OrdersView().SingleOrDefault(t => t.ID == id);
            }
        }

        protected void complete()
        {
            try
            {
                this.Order.Completed();
                Response.Write(true);
            }
            catch (Exception ex)
            {
                Response.Write(false);
            }
            Response.End();

        }

        protected void close()
        {
            try
            {
                this.Order.Close();
                Response.Write(true);
            }
            catch (Exception ex)
            {
                Response.Write(false);
            }
            Response.End();
        }

    }
}