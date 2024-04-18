using Needs.Ccs.Services.Enums;
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
    ///已出库出库通知
    /// 香港库房
    /// </summary>
    public partial class ExitedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载出库通知详情列表
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            string VoyNo = Request.QueryString["VoyNo"];
            var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice
                             .Where(x => x.ExitNoticeStatus == ExitNoticeStatus.Exited).AsQueryable();
            //查询条件
            if (!string.IsNullOrEmpty(OrderID))
            {
                exitNotices = exitNotices.Where(x => x.Order.ID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(VoyNo))
            {
                exitNotices = exitNotices.Where(x => x.DecHead.VoyNo.Contains(VoyNo));
            }

            exitNotices = exitNotices.OrderByDescending(x => x.CreateDate);

            // 返回数据列表
            Func<HKExitNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,//订单编号
                VoyNo = item.DecHead.VoyNo,
                ClientName = item.Order.Client.Company.Name,//客户
                PackNo = item.DecHead.PackNo,
               // AdminName = item.Admin.UserName,//制单人
                CreateDate = item.CreateDate.ToShortDateString(),
                NoticeStatus = item.ExitNoticeStatus.GetDescription(),
            };
            this.Paging(exitNotices, convert);
        }
    }
}