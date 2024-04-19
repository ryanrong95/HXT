using Needs.Erp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Contacts
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
                this.Model = ErpPlot.Current.Publishs.MyContacts[id];
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
           
            var contactid = Request.QueryString["ID"];          
            var names = Request.Form["txtName"];
            var sex = Request.Form["radioSex"];
            var birthday = Request.Form["txtBirthday"];
            var tels = Request.Form["txtTel"];
            var email = Request.Form["txtEmail"];
            var mobile = Request.Form["txtMobile"];
            var company = Request.Form["txtCompanyID"];
            var status = Request.Form["txtStatus"];
            var job = Request.Form["txtJob"];
            var entiry = ErpPlot.Current.Publishs.MyContacts[ID] ?? new NtErp.Vrs.Services.Models.Contact
            {
                Status = NtErp.Vrs.Services.Enums.Status.Nomal,
            };                            
            entiry.ID = contactid;           
            entiry.Name = Convert.ToString(names);
           
            entiry.Sex = bool.Parse(sex);
            entiry.Birthday = birthday;
            entiry.Tel = Convert.ToString(tels);
            entiry.Email = Convert.ToString(email);
            entiry.Mobile = Convert.ToString(mobile);
            entiry.CompanyID =company;
            entiry.Status = (NtErp.Vrs.Services.Enums.Status)int.Parse(status);
            entiry.Job = (NtErp.Vrs.Services.Enums.JobType)int.Parse(job);
            entiry.EnterSuccess += EnterSuccess;
            entiry.Enter();
        
        }
        private void EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            base.Alert("保存成功",this.Request.Url);
        }
        protected object selects_company()
        {
            return ErpPlot.Current.Publishs.CompaniesAll.Select(item => new { item.ID, item.Name });
        }
    }
}