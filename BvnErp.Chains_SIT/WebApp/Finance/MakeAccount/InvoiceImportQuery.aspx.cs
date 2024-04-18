using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class InvoiceImportQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string invoiceNo = Request.QueryString["InvoiceNo"];
            string comanyName = Request.QueryString["ComanyName"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

         
            using (var query = new Needs.Ccs.Services.Views.InvoiceCredentialView())
            {
                var view = query;
                view = view.SearchByNotNullInvoiceNo();
                view = view.SearchByCreSta(true);

                if (!string.IsNullOrEmpty(invoiceNo))
                {
                    view = view.SearchByinvoiceNo(invoiceNo);
                }
                if (!string.IsNullOrEmpty(comanyName))
                {
                    view = view.SearchBycomanyName(comanyName);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    startDate = startDate.Trim();
                    DateTime dtStart = Convert.ToDateTime(startDate);
                    view = view.SearchByFrom(dtStart);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    endDate = endDate.Trim();
                    DateTime dtEnd = Convert.ToDateTime(endDate).AddDays(1);
                    view = view.SearchByTo(dtEnd);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }

        }
    }
}