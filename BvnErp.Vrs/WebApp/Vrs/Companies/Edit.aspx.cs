using Needs.Erp;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Web;
using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Companies
{
    public partial class Edit : Needs.Web.Forms.ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PageInit();
            }
        }
        void PageInit()
        {
            var id = Request.QueryString["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                this.Model = ErpPlot.Current.Publishs.CompaniesAll[id];
            }
        }
        protected void GetCompany()
        {
            var id = Request["txtName"].MD5();
            NtErp.Vrs.Services.Models.Company data = ErpPlot.Current.Publishs.CompaniesAll[id]??new NtErp.Vrs.Services.Models.Company();
            Response.Write(data.Json());
            Response.End();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var name = Request["txtName"];
            var type = Request["select_type"];
            var code = Request["txtCode"];
            var company_address = Request["txtAddress"];
            var company_num = Request["txtRegisteredCapital"];
            var company_corporate = Request["txtCorporateRepresentative"];
            var summary = Request["txtSummary"];
            var entiry = ErpPlot.Current.Publishs.CompaniesAll[id] ?? new NtErp.Vrs.Services.Models.Company();
            entiry.Name = name;
            entiry.Type = (ComapnyType)int.Parse(type);
            entiry.Code = code;
            entiry.Address = company_address;
            entiry.RegisteredCapital = company_num;
            entiry.CorporateRepresentative = company_corporate;
            entiry.Summary = summary;
            entiry.EnterSuccess += EnterSuccess;
            entiry.Enter();
        }
        private void EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            base.Alert("保存成功", this.Request.Url);
        }
    }
}