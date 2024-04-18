using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.ReportManage.jumuReport
{
    public partial class JimuTemplate : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                reportInfo();
            }
        }

        protected void reportInfo()
        {
            string jimuURL = System.Configuration.ConfigurationManager.AppSettings["JimuReport"];
            string adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            var country = new JimuReportView().Where(t => t.AdminID == adminID).AsQueryable();
            List<jiMushow> results = new List<jiMushow>();
            foreach(var item in country)
            {
                jiMushow jiMushow = new jiMushow();
                jiMushow.reportName = item.ReportName;
                jiMushow.reportUrl = jimuURL+item.ReportUrl;
                results.Add(jiMushow);
            }


            this.Model.urls = results.Json();
        }

        public class jiMushow
        {
            public string reportName { get; set; }
            public string reportUrl { get; set; }
        }
    }
}