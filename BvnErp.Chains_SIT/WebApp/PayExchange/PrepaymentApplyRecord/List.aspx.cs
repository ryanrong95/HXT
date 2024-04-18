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

namespace WebApp.PayExchange.PrepaymentApplyRecord
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            this.Model.PayExchangeApplyStatus = EnumUtils.ToDictionary<PayExchangeApplyStatus>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string PayExchangeApplyID = Request.QueryString["ID"];
            string ClientName = Request.QueryString["ClientName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.PrePayExchangeAppliesView())
            {
                var view = query;

                view = view.SearchByClientAdmin(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                if (!string.IsNullOrEmpty(PayExchangeApplyID))
                {
                    view = view.SearchByPayExchangeApplyID(PayExchangeApplyID);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    view = view.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    var start = Convert.ToDateTime(StartDate);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var end = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByEndDate(end);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
        protected void Matching()
        {
            string OrderID = Request.Form["OrderID"];

            var controls = new Needs.Ccs.Services.Views.PrepaymentApplyListView().Where(t => t.OrderID == OrderID).ToList();
            if (controls.Count <= 1)
            {
                Response.Write((new { success = true, message = "没有匹配到数据" }).Json());
            }
            else
            {
                Response.Write((new { success = true, message = "" }).Json());
            }
        }
    }
}