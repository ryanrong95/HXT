using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using Needs.Web;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Oss.Orders
{
    /// <summary>
    /// 一切ajax 失败直接靠 异常 来操作，超时就是超时。不要混淆
    /// </summary>
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { }
        }

        protected void data()
        {
            Expression<Func<Order, bool>> exp = item => true;

            var orderid = Request["orderid"];
            var user = Request["user"];
            NtErp.Wss.Oss.Services.OrderStatus status;
            var views = Needs.Erp.ErpPlot.Current.OrderSales.MyOrders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(orderid))
            {
                views = views.Where(item => item.ID.Contains(orderid));
            }
            if (!string.IsNullOrWhiteSpace(user))
            {
                views = views.Where(item => item.Client.ID.Contains(user) || item.Client.UserName.Contains(user));
            }
            if (Enum.TryParse(Request["status"], out status) && (int)status != -1)
            {
                views = views.Where(item => item.Status == status);
            }


            Response.Paging(views.OrderByDescending(t => t.CreateDate), item => new
            {
                ID = item.ID,
                UserID = item.Client.ID,
                UserName = item.Client.UserName,
                District = Needs.Underly.Legally.Current[item.Consignee.District].ShortName,
                Currency = Needs.Underly.Legally.Current[item.Beneficiary.Currency].ShortName,
                Transport = item.TransportTerm.Carrier,
                StatusStr = item.Status.GetDescription(),
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                item.Status,
                item.Total,
                item.Paid,
                Unpaid = item.Total - item.Paid
            });

        }

        /// <summary>
        /// 订单-完成
        /// </summary>
        protected void complete()
        {
            try
            {
                var oid = Request["orderid"];
                Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[oid].Complete();
                Response.Write(true);
            }
            catch (Exception ex)
            {
                Response.Write(false);
            }
            Response.End();

        }
        /// <summary>
        /// 订单-关闭
        /// </summary>
        protected void close()
        {
            try
            {
                var oid = Request["orderid"];
                Needs.Erp.ErpPlot.Current.OrderSales.MyOrders[oid].Close();
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