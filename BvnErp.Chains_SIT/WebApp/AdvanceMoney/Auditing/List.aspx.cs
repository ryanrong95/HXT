using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AdvanceMoney.Auditing
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
            this.Model.AdvanceMoneyStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AdvanceMoneyStatus>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").
              Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();
            this.Model.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string Status = Request.QueryString["Status"];
            string ServiceManager = Request.QueryString["ServiceManager"];
            using (var query = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView())
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
                    int advanceMoneyStatus = Convert.ToInt32(Status);
                    view = view.SearchByApplyStatus(advanceMoneyStatus);
                }
                if (!string.IsNullOrEmpty(ServiceManager))
                {
                    view = view.SearchByServiceManager(ServiceManager);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}
