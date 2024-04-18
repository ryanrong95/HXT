using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Reports
{
    public partial class DetailList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
            }
        }

        protected void data()
        {
            string ActionID = Request.QueryString["ID"];
           // var reports = Needs.Erp.ErpPlot.Current.ClientSolutions.Reports;
            //var report = reports.Where(item => item.ActionID == ActionID);
            //this.Paging(report);

           // var report = reports.Where(item => item.ActionID == ActionID);

            Func<NtErp.Crm.Services.Models.Report, object> linq = item => new
            {
                ID = item.ID,
                Context = item.Context,
            };
            //this.Paging(report, linq);
        }
    }
}