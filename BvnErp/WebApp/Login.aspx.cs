using Needs.Settings;
using Needs.Utils.Http;
using Needs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp
{
    public partial class Login : Needs.Web.Forms.ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var admin = new Needs.Erp.Models.ErpAdmin
            {
                UserName = this.txtUserName.Value.Trim(),
                Password = this.hPassword.Value.Trim()
            };
            admin.LoginSuccess += Admin_LoginSuccess;
            admin.LoginFailed += Admin_LoginFailed;
            admin.Login();
        }

        private void Admin_LoginFailed(object sender, Needs.Linq.ErrorEventArgs e)
        {
            base.Alert(this.hLoginningFail.Value);
        }

        private void Admin_LoginSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Redirect($"{nameof(Panels)}.aspx");
        }
    }
}