using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order
{
    /// <summary>
    /// 订单退回或取消的原因
    /// </summary>
    public partial class Reason : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 订单退回或取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_ServerClick(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            string source = Request.QueryString["Source"];
            string reason = Request.Form["reason"];

            if (source == "Cancel")
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyReturnedOrders[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                order.SetAdmin(admin);
                order.CanceledSummary = reason;
                order.Canceled += Order_CancelSuccess;
                order.Cancel();
            }
            else if (source == "Return")
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                order.SetAdmin(admin);
                order.ReturnedSummary = reason;
                order.Returned += Order_ReturnSuccess;
                order.Return();
            }
        }

        /// <summary>
        /// 订单取消成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_CancelSuccess(object sender, OrderCanceledEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("取消成功", url, true);
        }

        /// <summary>
        /// 订单退回成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_ReturnSuccess(object sender, OrderReturnedEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("退回成功", url, true);
        }
    }
}