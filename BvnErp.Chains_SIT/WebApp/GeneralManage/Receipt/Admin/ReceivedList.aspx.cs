using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;

namespace WebApp.GeneralManage.Receipt.Admin
{
    /// <summary>
    /// 管理员的已收款查询界面
    /// </summary>
    public partial class ReceivedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //Roles（Name=“业务员”）-> AdminRoles-> RealName
            this.Model.Salesman = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(s => s.Role.Name == "业务员").Select(t => new
            {
                Value = t.Admin.ID,
                Text = t.Admin.RealName
            }).Json();
        }

        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderId"];
            var Salesman = Request.QueryString["Salesman"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceivedNew.AsQueryable();

            if (!string.IsNullOrEmpty(ClientCode))
            {
                orderReceipts = orderReceipts.Where(o => o.ClientCode == ClientCode.Trim());
            }
            if (!string.IsNullOrEmpty(Salesman))
            {
                orderReceipts = orderReceipts.Where(o => o.Salesman == Salesman.Trim());
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                orderReceipts = orderReceipts.Where(o => o.ID == OrderId.Trim());
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                orderReceipts = orderReceipts.Where(t => t.DeclareDate >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {

                var to = DateTime.Parse(EndDate);
                orderReceipts = orderReceipts.Where(item => item.DeclareDate < to.AddDays(1));
            }

            orderReceipts = orderReceipts.OrderByDescending(item => item.DeclareDate);

            Func<Needs.Ccs.Services.Models.OrderReceiptInfo, object> convert = t => new
            {
                ClientCode = t.ClientCode,
                CompanyName = t.CompanyName,
                OrderId = t.ID,
                OrderStatus = t.OrderStatus.GetDescription(),
                DeclareDate = t.DeclareDate?.ToShortDateString(),
                DeclarePrice = t.DeclarePrice.ToString("0.00"),
                Received = t.Received,
                //Profit = t.Profit.ToString("0.00"),
                Salesman = t.Salesman
            };

            this.Paging(orderReceipts, convert);
        }
    }
}