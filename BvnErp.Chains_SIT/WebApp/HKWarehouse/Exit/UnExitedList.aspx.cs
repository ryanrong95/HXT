using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Exit
{
    /// <summary>
    /// 出库通知列表操作
    /// 香港库房
    /// </summary>
    public partial class UnExitedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 香港出库通知（未出库）
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            string VoyNo = Request.QueryString["VoyNo"];

            var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice
                             .Where(x => x.ExitNoticeStatus != ExitNoticeStatus.Exited).AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                exitNotices = exitNotices.Where(x => x.Order.ID.Contains(OrderID));
            }

            if (!string.IsNullOrEmpty(VoyNo))
            {
                exitNotices = exitNotices.Where(x => x.DecHead.VoyNo.Contains(VoyNo));
            }

            exitNotices = exitNotices.OrderByDescending(x => x.CreateDate);

            Func<HKExitNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,
                VoyNo = item.DecHead.VoyNo,
                ClientName = item.Order.Client.Company.Name,
                PackNo = item.DecHead.PackNo,
                CreateDate = item.CreateDate.ToShortDateString(),
                NoticeStatus = item.ExitNoticeStatus.GetDescription(),
            };
            this.Paging(exitNotices, convert);
        }

        /// <summary>
        /// 一键出库
        /// </summary>
        protected void OutStock()
        {
            try
            {
                var Ids = Request["IDs"].Split(',');
                var ExitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice.Where(item => Ids.Contains(item.ID)).ToList();
                foreach(var ExitNotice in ExitNotices)
                {
                    ExitNotice.OutStock();
                }
                Response.Write((new { success = true, message = "出库成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }

    }
}