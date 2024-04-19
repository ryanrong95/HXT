using Needs.Erp;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Invoices
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                initData();
            }
        }
        void initData()
        {
            var id = Request.QueryString["id"];
            this.Model.Invoice = ErpPlot.Current.Publishs.InvoicesAll[id];
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            string required = Request["radioRequired"];
            string type = Request["select_type"];
            string companyid = Request["cbbCompany"];
            string contactid = Request["cbbContact"];
            //string address = Request.QueryString["txtAddress"];
            string bank = Request["txtBank"];
            string bankaddress = Request["txtAddress"]; ;
            string account = Request["txtAccount"];
            var entity = ErpPlot.Current.Publishs.InvoicesAll[id] ?? new NtErp.Vrs.Services.Models.Invoice();
            entity.Required = bool.Parse(required);
            entity.Type = (InvoiceType)int.Parse(type);
            entity.CompanyID = companyid;
            entity.ContactID = contactid;
            // entity.Address = address;
            entity.Bank = bank;
            entity.BankAddress = bankaddress;
            entity.Account = account;
            entity.EnterSuccess += EnterSuccess;
            entity.Enter();

        }
        private void EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            base.Alert("保存成功", this.Request.Url);
        }
        protected object selects_company()
        {
            return ErpPlot.Current.Publishs.CompaniesAll.Select(item => new { item.ID, item.Name });
        }
        protected object selects_contact()
        {
            return ErpPlot.Current.Publishs.MyContacts.Select(item => new { item.ID, item.Name });
        }
    }
}