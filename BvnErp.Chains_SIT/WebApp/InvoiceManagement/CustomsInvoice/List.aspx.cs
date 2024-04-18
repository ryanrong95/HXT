using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InvoiceManagement.CustomsInvoice
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

            //所属月份
            string DeductionMonth = Request.QueryString["DeductionMonth"];
            //海关缴款书号码
            string TaxNumber = Request.QueryString["TaxNumber"];
            //海关票日期：
            string PayDateFrom = Request.QueryString["PayDateFrom"];
            string PayDateTo = Request.QueryString["PayDateTo"];
         

            var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.CustomsInvoiceView.CustomsInvoiceViewModels>();

            if (!string.IsNullOrEmpty(DeductionMonth))
            {
                DeductionMonth = DeductionMonth.Trim()+"-01";
                DateTime dtDeductionMonth = Convert.ToDateTime(DeductionMonth);
                predicate = predicate.And(item => item.DeductionMonth==dtDeductionMonth);
            }
            if (!string.IsNullOrEmpty(TaxNumber))
            {
                TaxNumber = TaxNumber.Trim();               
                predicate = predicate.And(item => item.TaxNumber == TaxNumber);
            }

            if (!string.IsNullOrEmpty(PayDateFrom))
            {
                PayDateFrom = PayDateFrom.Trim();
                DateTime dtPayDateFrom = Convert.ToDateTime(PayDateFrom);              
                predicate = predicate.And(item => item.CustomsInvoiceDate>= dtPayDateFrom);
            }
           

            if (!string.IsNullOrEmpty(PayDateTo))
            {
                PayDateTo = PayDateTo.Trim();
                DateTime dtPayDateTo = Convert.ToDateTime(PayDateTo);
                predicate = predicate.And(item => item.CustomsInvoiceDate <= dtPayDateTo);
            }

           

            Needs.Ccs.Services.Views.CustomsInvoiceView view = new Needs.Ccs.Services.Views.CustomsInvoiceView();
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;         
            view.Predicate = predicate;

            int recordCount = view.RecordCount;
            var decTaxs = view.ToList();

            Func<Needs.Ccs.Services.Views.CustomsInvoiceView.CustomsInvoiceViewModels, object> convert = decTax => new
            {
                CustomsInvoiceDate = decTax.CustomsInvoiceDate==null?"":decTax.CustomsInvoiceDate.Value.ToString("yyyy-MM-dd"),
                TaxNumber = decTax.TaxNumber,
                Amount = decTax.Amount,
                VaildAmount = decTax.VaildAmount,
                DeductionMonth = decTax.DeductionMonth==null?"":decTax.DeductionMonth.Value.ToString("yyyy-MM")
            };

            Response.Write(new
            {
                rows = decTaxs.Select(convert).ToArray(),
                total = recordCount,
            }.Json());
        }
    }
}