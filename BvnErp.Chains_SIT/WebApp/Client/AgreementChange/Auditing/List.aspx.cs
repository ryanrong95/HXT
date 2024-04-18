using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.AgreementChange.Auditing
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
            this.Model.Status = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AgreementChangeApplyStatus>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];

            string Status = Request.QueryString["Status"];
            using (var query = new Needs.Ccs.Services.Views.AgreementChangeApplyListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    view = view.SearchByClientCode(ClientCode);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    view = view.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    int agreementChangeApplyStatus = Convert.ToInt32(Status);
                    view = view.SearchByApplyStatus(agreementChangeApplyStatus);
                }
                if (!string.IsNullOrEmpty(CreateDateFrom))
                {
                    var from = DateTime.Parse(CreateDateFrom);
                    view = view.SearchByCreateDateFrom(from);
                }
                if (!string.IsNullOrEmpty(CreateDateTo))
                {
                    var to = DateTime.Parse(CreateDateTo).AddDays(1);
                    view = view.SearchByCreateDateTo(to);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}
