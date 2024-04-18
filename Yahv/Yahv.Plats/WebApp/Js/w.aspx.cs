using System;
using System.Web;
using Yahv.Web.Forms;

namespace WebApp.Js
{
    public partial class w : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var code = HttpContext.Current.Request.Params["code"];
            string state = HttpContext.Current.Request.Params["state"];
            var user = new Yahv.Plats.Services.LoginUser();

            //区别绑定还是登录
            if (string.IsNullOrWhiteSpace(Request["IsBind"]))
            {
                user.LoginSuccess += User_LoginSuccess;
                user.Unbound += User_Unbound;
                user.LoginClosed += User_LoginClosed;
                user.RoleErrorClosed += User_RoleErrorClosed;
                user.LoginWXCallBack(code, state);
            }
            else if (Request["IsBind"] == "1")
            {
                user.BindSuccess += User_LoginSuccess;
                user.BindFailed += User_BindFailed;

                if (Yahv.Erp.Current != null && !string.IsNullOrWhiteSpace(Yahv.Erp.Current.StaffID))
                {
                    user.WxBind(code, Yahv.Erp.Current.StaffID);
                }
                else
                {
                    Easyui.Redirect("/Errors/WxBind.aspx");
                }
            }
        }

        private void User_RoleErrorClosed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            Easyui.Redirect("/Errors/Roles.aspx");
        }

        private void User_Unbound(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            Easyui.Redirect("/Errors/WxUnbound.aspx");
        }

        private void User_BindFailed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            Easyui.Redirect("/Errors/WxRepeatBind.aspx");
        }

        private void User_LoginClosed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            Easyui.Redirect("/Errors/LoginClosed.aspx");
        }

        private void User_LoginSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            Easyui.Redirect("/Panels.aspx");
        }
    }
}