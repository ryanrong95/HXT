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
    public partial class DeclareImport : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //开票类型
            this.Model.InvoiceTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string invoiceType = Request.QueryString["InvoiceType"];
            string companyName = Request.QueryString["CompanyName"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            using (var query = new Needs.Ccs.Services.Views.MKDeclareImportView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(invoiceType))
                {
                    int type = int.Parse(invoiceType);
                    view = view.SearchByInvoiceType((Needs.Ccs.Services.Enums.InvoiceType)type);
                }
                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                    view = view.SearchByName(companyName);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    var from = DateTime.Parse(startDate);
                    view = view.SearchByFrom(from);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var to = DateTime.Parse(endDate).AddDays(1);
                    view = view.SearchByTo(to);
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
            var  model = Model.JsonTo<List<SubjectReportStatistics>>();

            var result = new Needs.Ccs.Services.Models.DeclareImport(model).Make();

            Response.Write((new { success = result}).Json());

        }

        protected void MakeAccountAll()
        {
            string invoiceType = Request.Form["InvoiceType"];
            string companyName = Request.Form["CompanyName"];
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];

            if(!string.IsNullOrEmpty(startDate)&& !string.IsNullOrEmpty(endDate))
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
                Response.Write((new { success = false,msg="必须勾选开始结束日期" }).Json());
                return;
            }

            using (var query = new Needs.Ccs.Services.Views.MKDeclareImportView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(invoiceType))
                {
                    int type = int.Parse(invoiceType);
                    view = view.SearchByInvoiceType((Needs.Ccs.Services.Enums.InvoiceType)type);
                }
                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                    view = view.SearchByName(companyName);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    var from = DateTime.Parse(startDate);
                    view = view.SearchByFrom(from);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var to = DateTime.Parse(endDate).AddDays(1);
                    view = view.SearchByTo(to);
                }

                IEnumerable<SubjectReportStatistics> data =  (IEnumerable<SubjectReportStatistics>)view.ForCredentialData();
                var model = data.ToList();

                var result = new Needs.Ccs.Services.Models.DeclareImport(model).Make();

                Response.Write((new { success = result }).Json());

            }
        }
    }
}