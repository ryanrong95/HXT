using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InvoiceManagement.ServiceInvoice
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

            string DeductionMonth = Request.QueryString["DeductionMonth"];
            string InvoiceNo = Request.QueryString["InvoiceNo"];
            string InvoiceDateFrom = Request.QueryString["InvoiceDateFrom"];
            string InvoiceDateTo = Request.QueryString["InvoiceDateTo"];
            string SellsName = Request.QueryString["SellsName"];

            var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.TaxManageView.TaxManageViewModels>();
            predicate = predicate.And(item => item.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Service);

            if (!string.IsNullOrEmpty(DeductionMonth))
            {
                DeductionMonth = DeductionMonth.Trim() + "-01";
                DateTime dtDeductionMonth = Convert.ToDateTime(DeductionMonth);
                predicate = predicate.And(item => item.AuthenticationMonth == dtDeductionMonth);
            }
            if (!string.IsNullOrEmpty(InvoiceNo))
            {
                InvoiceNo = InvoiceNo.Trim();
                predicate = predicate.And(item => item.InvoiceNo == InvoiceNo);
            }
            if (!string.IsNullOrEmpty(SellsName))
            {
                SellsName = SellsName.Trim();
                predicate = predicate.And(item => item.SellsName.Contains(SellsName));
            }

            if (!string.IsNullOrEmpty(InvoiceDateFrom))
            {
                InvoiceDateFrom = InvoiceDateFrom.Trim();
                DateTime dtPayDateFrom = Convert.ToDateTime(InvoiceDateFrom);
                predicate = predicate.And(item => item.InvoiceDate >= dtPayDateFrom);
            }


            if (!string.IsNullOrEmpty(InvoiceDateTo))
            {
                InvoiceDateTo = InvoiceDateTo.Trim();
                DateTime dtPayDateTo = Convert.ToDateTime(InvoiceDateTo);
                predicate = predicate.And(item => item.InvoiceDate <= dtPayDateTo);
            }



            Needs.Ccs.Services.Views.TaxManageView view = new Needs.Ccs.Services.Views.TaxManageView();
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;
            view.Predicate = predicate;

            int recordCount = view.RecordCount;
            var decTaxs = view.ToList();

            Func<Needs.Ccs.Services.Views.TaxManageView.TaxManageViewModels, object> convert = decTax => new
            {
                InvoiceCode = decTax.InvoiceCode,
                InvoiceNo = decTax.InvoiceNo,
                InvoiceDate = decTax.InvoiceDate == null ? "" : decTax.InvoiceDate.Value.ToString("yyyy-MM-dd"),
                SellsName = decTax.SellsName,
                Amount = decTax.Amount,
                VaildAmount = decTax.VaildAmount,
                ConfrimDate = decTax.ConfrimDate == null ? "" : decTax.ConfrimDate.Value.ToString("yyyy-MM"),
                IsVaild = decTax.InvoiceStatus,
                InvoiceDetailID = decTax.InvoiceDetailID,
                AuthenticationMonth = decTax.AuthenticationMonth == null ? "" : decTax.AuthenticationMonth.Value.ToString("yyyy-MM"),
                //IsValidDesc = decTax.InvoiceStatus.GetDescription()
            };

            Response.Write(new
            {
                rows = decTaxs.Select(convert).ToArray(),
                total = recordCount,
            }.Json());
        }        
    }
}