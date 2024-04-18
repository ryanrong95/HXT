using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InvoiceManagement.InvoiceDetail
{
    public partial class InvoiceItems : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data() {
            
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ID = Request.QueryString["ID"];
            var declarantCandidatesView = new Needs.Ccs.Services.Views.InvoiceResultItemsView().
                Where(t=>t.InvoiceResultID==ID&&t.lineNum!=0).OrderBy(t=>t.lineNum).AsQueryable();

            Func<Needs.Ccs.Services.Models.InvoiceDetailData, object> convert = item => new
            {
                lineNum = item.lineNum,
                goodserviceName = item.goodserviceName,
                model = item.model,
                unit = item.unit,
                number = item.number,
                price = item.price,
                sum = item.sum,
                taxRate = item.taxRate,                
                tax = item.tax,
            };

            this.Paging(declarantCandidatesView, convert);
        }
    }
}