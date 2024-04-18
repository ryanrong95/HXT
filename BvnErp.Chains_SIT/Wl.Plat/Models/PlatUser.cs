using Layer.Data.Sqls;
using Needs.Utils.Converters;
using System;
using System.Linq;
using System.Web;

namespace Needs.Wl.User.Plat.Models
{
    /// <summary>
    /// Wl.net.cn 芯达通平台会员(PC端)
    /// </summary>
    public partial class PlatUser : IPlatUser, ILocalUser
    {
        public virtual event LoginSuccessHanlder LoginSuccess;
        public virtual event LoginFailedHanlder LoginFailed;

        public string ID { get; internal set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户的真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 微信OpenID
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// 是否主账号
        /// </summary>
        public bool IsMain { get; set; }

        private Needs.Wl.Models.Client client;

        public Needs.Wl.Models.Client Client
        {
            get
            {
                if (this.client == null)
                {
                    this.client = new Views.UserClientView(this).FirstOrDefault();
                }

                return this.client;
            }
            set
            {
                this.client = value;
            }
        }

        public bool isvalid;
        public bool IsValid
        {
            get
            {
                var valid = new Views.UserClientOrignView(this).FirstOrDefault().IsValid;
                this.isvalid = (valid.HasValue && valid.Value) ? true : false;
                return isvalid;
            }
            set
            {
                this.isvalid = value;
            }
        }

        /// <summary>
        /// 在站点中记住用户名密码，下次自动登录
        /// </summary>
        public bool RemberInWebSite { get; set; }

        public PlatUser()
        {
            this.LoginSuccess += PlatUser_LoginSuccess;
        }

        private void PlatUser_LoginSuccess(object sender, SuccessEventArgs e)
        {
            PlatUser user = e.Object as PlatUser;
            if (user == null)
            {
                throw new Exception("A serious mistake!");
            }

            var ip = Needs.Utils.HttpRequest.UserHostAddress;
            string token = string.Concat("$", user.ID, "*", "&", ip, "#").MD5("x");

            Plat.Identity.CreateUserToken(user, token, ip);
            Plat.Identity.ResponseCookie(user.UserName, token, user.RemberInWebSite);
        }

        public virtual void Login()
        {
            string password = this.Password.StrToMD5();

            var user = Plat.Identity.UserLogin(this.UserName, password);
            if (user != null && (string.IsNullOrEmpty(user.ID) == false))
            {
                this.ID = user.ID;
                this.Client = user.Client;
                HttpContext.Current.Session["User"] = user;

                this.OnLoginSuccess();
            }
            else
            {
                this.OnLoginFailed();
            }
        }

        protected virtual void OnLoginSuccess()
        {
            if (this != null && this.LoginSuccess != null)
            {
                this.LoginSuccess(this, new SuccessEventArgs(this));
            }
        }

        protected virtual void OnLoginFailed()
        {
            if (this != null && this.LoginFailed != null)
            {
                this.LoginFailed(this, new ErrorEventArgs("Login failure"));
            }
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="oldPassWord"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string oldPassWord, string newPassword)
        {
            using (var repository = new ScCustomsReponsitory())
            {
                newPassword = newPassword.StrToMD5();
                var single = repository.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().Single(item => item.ID == this.ID);
                if (single.Password != oldPassWord)
                {
                    return false;
                }
                repository.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Password = newPassword }, item => item.ID == this.ID);
                return true;
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="newPassword"></param>
        public void ResetPassword(string newPassword)
        {
            using (var repository = new ScCustomsReponsitory())
            {
                repository.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Password = newPassword }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 修改手机绑定
        /// </summary>
        /// <param name="newMobile">新手机号码</param>
        public bool ChangeMobile(string newMobile)
        {
            using (var repository = new ScCustomsReponsitory())
            {
                var single = repository.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().SingleOrDefault(item => item.ID == this.ID);
                if (single == null)
                {
                    return false;
                }

                repository.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Mobile = newMobile }, item => item.ID == this.ID);
            }

            return true;
        }

        /// <summary>
        /// 修改邮箱绑定
        /// </summary>
        /// <param name="newMobile">新手机号码</param>
        public bool ChangeEmail(string newEmail)
        {
            using (var repository = new ScCustomsReponsitory())
            {
                var single = repository.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().SingleOrDefault(item => item.ID == this.ID);
                if (single == null)
                {
                    return false;
                }
                repository.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Email = newEmail }, item => item.ID == this.ID);
            }

            return true;
        }

        /// <summary>
        /// 修改登录名
        /// </summary>
        /// <param name="newPassword"></param>
        public bool ResetUserName(string newUserName)
        {
            using (var repository = new ScCustomsReponsitory())
            {
                var single = repository.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().SingleOrDefault(item => item.ID == this.ID);
                if (single == null)
                {
                    return false;
                }

                repository.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Name = newUserName }, item => item.ID == this.ID);
            }

            return true;
        }

        public virtual void Logout()
        {
            Plat.Identity.ClearCookie();
        }
    }
}