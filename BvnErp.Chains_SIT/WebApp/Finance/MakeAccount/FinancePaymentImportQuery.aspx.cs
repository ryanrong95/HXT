using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class FinancePaymentImportQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {


        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];



            using (var query = new Needs.Ccs.Services.Views.FinancePaymentViewRJ())
            {
                var view = query;

                view = view.SearchByCreSta(true);
                view = view.SearchByPayeeName("暂收款");

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate);
                    view = view.SearchByTo(to);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}