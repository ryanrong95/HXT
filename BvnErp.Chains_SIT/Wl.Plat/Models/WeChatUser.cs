using System;
using System.Web;

namespace Needs.Wl.User.Plat.Models
{
    /// <summary>
    /// 微信用户
    /// </summary>
    public partial class WeChatUser : PlatUser, IPlatUser, ILocalUser
    {
        public override event LoginSuccessHanlder LoginSuccess;
        public override event LoginFailedHanlder LoginFailed;

        public WeChatUser()
        {
            this.LoginSuccess += WeChatUser_LoginSuccess;
        }

        public override void Login()
        {
            base.Login();
        }

        protected override void OnLoginSuccess()
        {
            if (this != null && this.LoginSuccess != null)
            {
                this.LoginSuccess(this, new SuccessEventArgs(this));
            }
        }

        protected override void OnLoginFailed()
        {
            if (this != null && this.LoginFailed != null)
            {
                this.LoginFailed(this, new ErrorEventArgs("Login failure"));
            }
        }

        private void WeChatUser_LoginSuccess(object sender, SuccessEventArgs e)
        {
            var user = e.Object as WeChatUser;
            if (user == null)
            {
                throw new Exception("A serious mistake!");
            }

            HttpContext.Current.Session["User"] = user;
            WeChatIdentity.CreateUserOpenID(user, WeChatPlat.OpenID);
        }
    }
}
