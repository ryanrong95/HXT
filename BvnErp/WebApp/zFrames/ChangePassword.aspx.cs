using Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NtErp.Services.Models;

namespace WebApp.zFrames
{
    public partial class ChangePassword : Needs.Web.Sso.Forms.ErpPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.txtRealName.Text = Needs.Erp.ErpPlot.Current.RealName;
                this.txtUserName.Text = Needs.Erp.ErpPlot.Current.UserName;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var current = Needs.Erp.ErpPlot.Current as Needs.Erp.Models.IAdmin;

            //if (current.IsSa)
            //{
            //    Alert(this.NoUpdatePassowrd.Value);
            //}
            //else
            //{
                current.PasswordSuccess += Current_PasswordSuccess;
                current.PasswordError += Current_PasswordError;
                string older = holdpassword.Value.Trim();
                string newer = hpassword.Value.Trim();
                current.ChangePassword(older, newer);
            //}
        }

        private void Current_PasswordError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Alert(this.OldPassError.Value);
        }

        private void Current_PasswordSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert(this.hSuccessMsg.Value);
        }
    }
}