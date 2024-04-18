using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services;
using Needs.Utils.Serializers;

namespace WebApp.GeneralManage.Receipt.ServiceManager
{
    /// <summary>
    /// 业务经理的待收款查询界面
    /// </summary>
    public partial class UnReceiveList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderUnReceiveStats.AsQueryable();
            var salesmanID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            //页面上所有的欠款
            this.Model.Overdraft = orderReceipts.Where(t=>t.Client.ServiceManager.ID == salesmanID)
                                                .Sum(t => (decimal?)t.Overdraft).GetValueOrDefault().Json();
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>()
                .Where(item => item.Key == Needs.Ccs.Services.Enums.OrderStatus.Declared.GetHashCode().ToString() 
                || item.Key == Needs.Ccs.Services.Enums.OrderStatus.Completed.GetHashCode().ToString()
                || item.Key == Needs.Ccs.Services.Enums.OrderStatus.WarehouseExited.GetHashCode().ToString()).Select(item => new { item.Key, item.Value });
            this.Model.OrderStatus = orderStatus.Json();
        }
        protected void data1()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderId"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            var salesmanID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderUnReceiveStats.AsQueryable();

            //只能看当前业务员的信息
            orderReceipts = orderReceipts.Where(o => o.Client.ServiceManager.ID == salesmanID);

            if (!string.IsNullOrEmpty(ClientCode))
            {
                orderReceipts = orderReceipts.Where(o => o.Client.ClientCode == ClientCode.Trim());
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                orderReceipts = orderReceipts.Where(o => o.ID == OrderId.Trim());

            }

            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                orderReceipts = orderReceipts.Where(t => t.DDate >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate);
                orderReceipts = orderReceipts.Where(item => item.DDate < to.AddDays(1));
            }

            orderReceipts = orderReceipts.OrderByDescending(item => item.DDate);

            Func<Needs.Ccs.Services.Models.OrderReceiptStats, object> convert = t => new
            {
                ClientCode = t.Client.ClientCode,
                CompanyName = t.Client.Company.Name,
                OrderId = t.ID,
                OrderStatus = t.OrderStatus.GetDescription(),
                DeclareDate = t.DDate?.ToShortDateString(),
                DeclarePrice = t.RMBDeclarePrice.ToRound(2).ToString("0.00"),
                Receivable = t.Receivable,
                Received = t.Received,
                Overdraft = t.Overdraft
            };

            this.Paging(orderReceipts, convert);
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderId"];
            string OrderStatus = Request.QueryString["OrderStatus"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            string strIsOnlyShowOverDate = Request.QueryString["IsOnlyShowOverDate"];
            bool isOnlyShowOverDate = false;
            if (!string.IsNullOrEmpty(strIsOnlyShowOverDate))
            {
                bool.TryParse(strIsOnlyShowOverDate, out isOnlyShowOverDate);
            }

            var salesmanID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            using (var query = new Needs.Ccs.Services.Views.OrderUnReceiveStatsViewNew())
            {
                var view = query;

                view = view.SearchByServiceManagerOrginAdminID(salesmanID);

                if (!string.IsNullOrWhiteSpace(ClientCode))
                {
                    ClientCode = ClientCode.Trim();
                    view = view.SearchByClientCode(ClientCode);
                }

                if (!string.IsNullOrWhiteSpace(OrderId))
                {
                    OrderId = OrderId.Trim();
                    view = view.SearchByOrderID(OrderId);
                }
                if (!string.IsNullOrEmpty(OrderStatus))
                {
                    int status = Int32.Parse(OrderStatus);
                    view = view.SearchByOrderStatus(status);
                }

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    DateTime begin = DateTime.Parse(StartDate);
                    view = view.SearchByDeclareDateBegin(begin);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    DateTime end = DateTime.Parse(EndDate);
                    end = end.AddDays(1);
                    view = view.SearchByDeclareDateEnd(end);
                }

                if (isOnlyShowOverDate)
                {
                    view = view.SearchByIsOverDue(isOverDue: true);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

    }
}