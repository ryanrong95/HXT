using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance
{
    public partial class DBSAccountFlow : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboboxData();
        }

        protected void LoadComboboxData()
        {
            this.Model.StartDate = DateTime.Now.AddDays(-7).ToShortDateString();
            this.Model.EndDate = DateTime.Now.ToShortDateString();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            
            string AccountNo = Request.QueryString["AccountNo"];
            string drCrInd = Request.QueryString["drCrInd"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
           

            using (var query = new Needs.Ccs.Services.Views.DBS.DBSAccountFlowView())
            {
                var view = query;
                if (!string.IsNullOrEmpty(AccountNo))
                {
                   
                    view = view.SearchByAccountNo(AccountNo);
                }
                if (!string.IsNullOrEmpty(drCrInd))
                {
                   
                    view = view.SearchByType(drCrInd);
                }
               
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByEndDate(end);
                }
                
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}