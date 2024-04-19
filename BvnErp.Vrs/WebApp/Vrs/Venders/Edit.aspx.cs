using Needs.Erp;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Venders
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
                this.Model = ErpPlot.Current.Publishs.MyVenders[id];
            }
            //this.txtName.Value = 
        }
        protected void GetCompany()
        {
            var id = Request["txtName"].MD5();
            var infor = ErpPlot.Current.Publishs.MyVenders[id].Json();
            if (infor == "null")
            {
                infor = ErpPlot.Current.Publishs.CompaniesAll[id].Json();
            }
            Response.Write(infor);
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.Form["txtName"].MD5();
            var type = Request.Form["select_type"];
            var grade = Request.Form["select_grade"];
            var company = ErpPlot.Current.Publishs.CompaniesAll[id];
            var entity = ErpPlot.Current.Publishs.MyVenders[id] ?? new NtErp.Vrs.Services.Models.Vender
            {
                Status = NtErp.Vrs.Services.Enums.Status.Nomal,
            };
            entity.ID = id;
            entity.Grade = (NtErp.Vrs.Services.Enums.Grade)int.Parse(grade);
            entity.Type = (NtErp.Vrs.Services.Enums.ComapnyType)int.Parse(type);
            entity.Properties = "";
            entity.Name = company.Name;
            entity.Address = company.Address;
            entity.RegisteredCapital = company.RegisteredCapital;
            entity.CorporateRepresentative = company.CorporateRepresentative;
            entity.EnterVenderSuccess += Vender_EnterSuccess;
            entity.Enter();
        }
        private void Vender_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("保存成功");
        }
    }
}