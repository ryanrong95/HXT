using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Sorting
{
    public partial class Log : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var orderId = Request.QueryString["OrderID"];
            var entryNoticeId = Request.QueryString["EntryNoticeID"];

            //产品变更日志
            var itemChangeLogs = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItemChangeLogs.Where(log => log.OrderID == orderId &&log.Summary.Contains("库房管理员"))
                                 .Select(log => new { ID= log.ID,CreateDate= log.CreateDate,Summary = log.Summary.Replace("\"","%22").Replace("\'","%27") }).ToArray();
            //订单库房费用日志
            var feeLogs = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumLogs.Where(log => log.OrderID == orderId)
                          .Select(log => new { log.ID, log.CreateDate, log.Summary }).ToArray();
            //入库通知日志
            var noticeLogs = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.EntryNoticeLogs.Where(log => log.EntryNoticeID == entryNoticeId)
                             .Select(log => new { log.ID, log.CreateDate, log.Summary }).ToArray();

            this.Model.Logs = (itemChangeLogs.Union(feeLogs).Union(noticeLogs)).OrderByDescending(log => log.CreateDate).Json();
        }
    }
}