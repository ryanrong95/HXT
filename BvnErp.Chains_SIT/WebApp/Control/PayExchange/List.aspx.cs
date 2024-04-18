using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.PayExchange
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

            string ClientCode = Request.QueryString["ClientCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.GendanAuditePayExchangeApplyListView())
            {
                var view = query;

                view = view.SearchByPayExchangeApplyStatus_LargerEqual(Needs.Ccs.Services.Enums.PayExchangeApplyStatus.Audited);

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    view = view.SearchByClientCode(ClientCode);
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

    }
}