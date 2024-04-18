using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.NoticeBoard
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.AdminID = "";
            //if (Needs.Wl.Admin.Plat.AdminPlat.Current.ID == ConfigurationManager.AppSettings["ApproveManID"])
            //{
            //    this.Model.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            //}
        }

        protected void NoticeData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string CreateDateBegin = Request.QueryString["CreateDateBegin"];
            string CreateDateEnd = Request.QueryString["CreateDateEnd"];
            string AdminID = Request.QueryString["AdminID"];

            using (var query = new Needs.Ccs.Services.Views.NoticeBoardView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(AdminID))
                {
                    view = view.SearchByClientAdminID(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                }
                else
                {
                    view = view.SearchByClientAdmin(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                }
                if (!string.IsNullOrEmpty(CreateDateBegin))
                {
                    var start = Convert.ToDateTime(CreateDateBegin);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(CreateDateEnd))
                {
                    var end = Convert.ToDateTime(CreateDateEnd).AddDays(1);
                    view = view.SearchByEndDate(end);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}