using Needs.Erp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Beneficiaries
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
                this.Model = ErpPlot.Current.Publishs.MyBeneficiaries[id];
            }

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            var id = Request.QueryString["ID"];
            var bank = Request.Form["txtBank"];
            var method = Request.Form["txtMethod"];
            var currency = Request.Form["txtCurrency"];
            var address = Request.Form["txtAddress"];
            var swiftcode = Request.Form["txtSwiftCode"];
            var contactid = Request.Form["txtContactID"];
            var company = Request.Form["txtCompanyID"];
            var status = Request.Form["txtStatus"];
            var entiry = ErpPlot.Current.Publishs.MyBeneficiaries[ID] ?? new NtErp.Vrs.Services.Models.Beneficiary
            {
                Status = NtErp.Vrs.Services.Enums.Status.Nomal,
            };
            entiry.ID = id;
            entiry.Bank = Convert.ToString(bank);
            entiry.Method =(NtErp.Vrs.Services.Enums.PayMethod)int.Parse(method);
            entiry.Currency =(Needs.Underly.Currency) int.Parse(currency);
            entiry.Address = Convert.ToString(address);
            entiry.SwiftCode = Convert.ToString(swiftcode);
            entiry.ContactID = contactid;
            entiry.CompanyID = company;
            entiry.Status = (NtErp.Vrs.Services.Enums.Status)int.Parse(status);
            entiry.EnterSuccess += EnterSuccess;
            entiry.Enter();

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