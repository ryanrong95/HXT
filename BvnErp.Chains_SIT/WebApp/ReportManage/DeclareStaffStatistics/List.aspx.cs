using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.ReportManage.DeclareStaffStatistics
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];


            using (var query = new Needs.Ccs.Services.Views.DeclareStaffStatisticsView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(StartTime))
                {
                    StartTime = StartTime.Trim();
                    var from = DateTime.Parse(StartTime);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrEmpty(EndTime))
                {
                    EndTime = EndTime.Trim();
                    var to = DateTime.Parse(EndTime).AddDays(1);
                    view = view.SearchByTo(to);
                }

                Response.Write(view.ToMyPage().Json());
            }
        }
    }
}