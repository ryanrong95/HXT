using Needs.Ccs.Services.Models;
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
    public partial class InvoiceImport : Uc.PageBase
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
                view = view.SearchByCreSta(false);

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

        /// <summary>
        /// 生成凭证
        /// </summary>
        protected void MakeAccount()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<InvoiceReportItem>>();

            var result = new Needs.Ccs.Services.Models.InvoiceImport(model).Make();

            Response.Write((new { success = result }).Json());

        }

        protected void MakeAccountAll()
        {
            string invoiceNo = Request.Form["InvoiceNo"];
            string comanyName = Request.Form["ComanyName"];
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];


            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime from = DateTime.Parse(startDate);
                DateTime to = DateTime.Parse(endDate).AddDays(1);
                TimeSpan day = to.Subtract(from);
                if (day.TotalDays > 31)
                {
                    Response.Write((new { success = false, msg = "不能一次生成超一个月的数据" }).Json());
                    return;
                }
            }
            else
            {
                Response.Write((new { success = false, msg = "必须勾选开始结束日期" }).Json());
                return;
            }

            var invoices = new Needs.Ccs.Services.Views.InvoiceCredentialView().
           Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal
                   && item.InvoiceNo != null
                   && item.InCreSta == false);

            if (!string.IsNullOrEmpty(invoiceNo))
            {
                invoiceNo = invoiceNo.Trim();
                invoices = invoices.Where(t => t.InvoiceNo == invoiceNo);
            }
            if (!string.IsNullOrEmpty(comanyName))
            {
                comanyName = comanyName.Trim();
                invoices = invoices.Where(t => t.Gfmc.Contains(comanyName));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                startDate = startDate.Trim();
                DateTime dtStart = Convert.ToDateTime(startDate);
                invoices = invoices.Where(t => t.InvoiceDate > dtStart);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                endDate = endDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(endDate).AddDays(1);
                invoices = invoices.Where(t => t.InvoiceDate < dtEnd);
            }

            List<InvoiceReportItem> model = new List<InvoiceReportItem>();

            foreach(var item in invoices)
            {
                model.Add(new InvoiceReportItem
                {
                    ID = item.ID,
                    InvoiceDate = item.InvoiceDate == null ? DateTime.Now : item.InvoiceDate.Value,
                    CompanyName = item.Gfmc,
                    InvoiceType = (int)item.InvoiceType
                });
            }

            var result = new Needs.Ccs.Services.Models.InvoiceImport(model).Make();

            Response.Write((new { success = result }).Json());

        }
    }
}