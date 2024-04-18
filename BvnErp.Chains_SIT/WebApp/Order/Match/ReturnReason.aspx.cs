using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Merchandiser
{
    /// <summary>
    /// 管控订单退回原因
    /// </summary>
    public partial class MatchReturnReason : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 审批不通过将订单退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_ServerClick(object sender, EventArgs e)
        {
            try
            {
                string id = Request.QueryString["ID"];
                string reason = Request.Form["reason"];

                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                order.SetAdmin(admin);
                order.ReturnedSummary = reason;
                order.Returned += Order_ReturnSuccess;
                order.Return();
            }
            catch (Exception ex)
            {
                this.Alert("订单退回失败：" + ex.Message);
            }
        }

        // <summary>
        /// 订单退回成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_ReturnSuccess(object sender, OrderReturnedEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("订单退回成功", url, true);
        }
    }
}