using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Plat.Administrators
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            id = Request.QueryString["id"] ?? "";
            if (!IsPostBack)
            {

                if (!string.IsNullOrEmpty(id))
                {
                    this.Model = Needs.Erp.ErpPlot.Current.Plots.Admins[id];
                    trPassword.Visible = false;
                    trNewPassword.Visible = true;
                }
                else
                {
                    trPassword.Visible = true;
                    trNewPassword.Visible = false;
                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var UserName = Request["UserName"];
            var RealName = Request["RealName"];
            var Summary = Request["Summary"];
            var entity = new NtErp.Services.Models.Admin { ID = id, UserName = UserName.Replace(" ", "").Trim(), RealName = RealName.Replace(" ", "").Trim(), Password = hPassword.Value, Summary = Summary };
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.EnterError += Entity_EnterError;
            entity.Enter();

            //throw new Exception();

        }

        private void Entity_EnterError_AccountRepeat(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Alert(this.AccountRepeat.Value, Request.Url);
        }

        private void Entity_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            if (e.Type == Needs.Linq.ErrorType.Repeated)
            {
                Alert(this.AccountRepeat.Value, Request.Url);
            }
            else
            {
                Alert(this.EnterError.Value, Request.Url);
            }
        }

        private void Entity_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert(this.EnterSuccess.Value, Request.Url, true);
        }
    }
}