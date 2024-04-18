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
    public partial class ReturnReason : Uc.PageBase
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
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                control.SetAdmin(admin);
                control.ReturnedSummary = reason;
                control.Returned += Control_ReturnSuccess;
                control.Return();
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
        private void Control_ReturnSuccess(object sender, OrderControledEventArgs e)
        {
            NoticeLog noticeLog = new NoticeLog();
            noticeLog.MainID = e.OrderControl.Order.ID;
            noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.ForbidRejected;
            noticeLog.Readed = true;
            noticeLog.SendNotice();

            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("订单退回成功", url, true);
        }
    }
}