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
    public partial class MerchandiserDetail : Uc.PageBase
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
            var MerchandiserId = Request.QueryString["ID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(x => x.Merchandiser.ID == MerchandiserId).Select(x => new { Key = x.ID, Value = x.Company.Name });
            this.Model.Clients = clients.Json();
            var Profits = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MerchandiserProfits[MerchandiserId];
            var from = DateTime.Parse(StartDate);
            var to = DateTime.Parse(EndDate);
            this.Model.ProfitDetails = new
            {

                // MerchandiserId = Profits.Name,
                StartDate = StartDate,
                EndDate = EndDate,
                // Profits = Profits.ProfitDetails.Where(x => x.DDate >= from && x.DDate < to.AddDays(1)).Sum(x => x.OrderProfit).ToRound(2)

            }.Json();
        }

        protected void data()
        {
            var MerchandiserId = Request.QueryString["ID"];
            string ClientID = Request.QueryString["ClientID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var ProfitDetails = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ProfitsDetails.Where(x => x.Client.Merchandiser.ID == MerchandiserId).AsQueryable();

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
                OrderID = t.ID,
                MerchandiserCommission = t.MerchandiserCommission.ToString("0.00"),// 提成
            };
            Response.Write(new { rows = ProfitDetails.Select(convert).ToArray() }.Json());
        }
    }
}