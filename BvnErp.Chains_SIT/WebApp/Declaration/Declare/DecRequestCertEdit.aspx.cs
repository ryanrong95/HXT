using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecRequestCertEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadcombobox();
            }
        }

        private void loadcombobox()
        {
            this.Model.DocuCode = Needs.Wl.Admin.Plat.AdminPlat.BaseDocuCode.Select(item => new { Value = item.Code, Text =  item.Name }).OrderBy(item=>item.Value).Json();           
        }

        protected void Save()
        {
            string DocuCode = Request.Form["DocuCode"];
            string CertCode = Request.Form["CertCode"];
            string DeclarationID = Request.Form["DeclarationID"];
            string ID = Request.Form["ID"];         
                     
            Needs.Ccs.Services.Models.DecLicenseDocu declicense = new Needs.Ccs.Services.Models.DecLicenseDocu();
            if (!string.IsNullOrEmpty(ID))
            {
                declicense.ID = ID;
            }
            else
            {
                declicense.DeclarationID = DeclarationID;
            }           
            declicense.DocuCode = DocuCode;
            declicense.CertCode = CertCode;
            declicense.EnterError += DecHead_EnterError;
            declicense.EnterSuccess += DecHead_EnterSuccess;          
            declicense.Enter();
            
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}