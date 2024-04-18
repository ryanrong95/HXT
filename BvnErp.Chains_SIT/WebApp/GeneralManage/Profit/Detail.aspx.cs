using Needs.Ccs.Services;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.Profit
{
    /// <summary>
    /// 利润提成详情界面
    /// </summary>

    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
            var SaleManId = Request.QueryString["ID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(x => x.ServiceManager.ID == SaleManId).Select(x => new { Key = x.ID, Value = x.Company.Name });
            this.Model.Clients = clients.Json();
            var Profits = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.Profits[SaleManId];
            var from = DateTime.Parse(StartDate);
            var to = DateTime.Parse(EndDate);
            this.Model.ProfitDetails = new
            {

                Salesman = Profits.Name,
                StartDate = StartDate,
                EndDate = EndDate,
                Profits = Profits.ProfitDetails.Where(x => x.DDate >= from && x.DDate < to.AddDays(1))
                                        .Sum(x => x.OrderProfit).ToRound(2),
                BusinessCommission = Profits.ProfitDetails.Where(x => x.DDate >= from && x.DDate < to.AddDays(1))
                                                   .Sum(x => x.Commission).ToRound(2),

            }.Json();
        }

        protected void data()
        {
            var SaleManId = Request.QueryString["ID"];
            string ClientID = Request.QueryString["ClientID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var ProfitDetails = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ProfitsDetails.Where(x => x.Client.ServiceManager.ID == SaleManId).AsQueryable();

            if (!string.IsNullOrEmpty(ClientID))
            {
                ProfitDetails = ProfitDetails.Where(o => o.Client.ID == ClientID.Trim());
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                ProfitDetails = ProfitDetails.Where(t => t.DDate >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate);
                ProfitDetails = ProfitDetails.Where(item => item.DDate < to.AddDays(1));
            }


            Func<Needs.Ccs.Services.Models.ProfitDetail, object> convert = t => new
            {
                ClientID = t.Client.Company.ID,
                ClientName = t.Client.Company.Name,
                RMBDeclarePrice = t.RMBDeclarePrice.ToString("0.00"),
                OrderDate = t.OrderDate.ToShortDateString(),
                ReceiveDate = t.ReceiveDate.ToShortDateString(),
                TaxGeneratTotal = t.TaxGeneratTotal.ToString("0.00"),
                FeeTotal = t.FeeTotal.ToString("0.00"),
                OrderID = t.ID,
                OrderProfits = t.OrderProfit.ToString("0.00"),//订单利润
                Proportion = t.proportion,//比例
                BusinessCommission = t.Commission.ToString("0.00"),// 业务提成
                CustomProfits = t.OrderProfit.ToString("0.00")
            };
            Response.Write(new { rows = ProfitDetails.Select(convert).ToArray() }.Json());
        }
    }
}