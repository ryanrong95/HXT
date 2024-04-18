using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.ReportManage.jumuReport
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            string adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            var country = new JimuReportView().Where(t=>t.AdminID== adminID).AsQueryable();

            this.Paging(country);
        }
    }
}