using System;
using System.Text.RegularExpressions;
using Yahv.Plats.Services.Models.Origins;
using Yahv.Utils.Http;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;


namespace WebApp
{
    public partial class Login : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PlatsUrlWriter.IsResolution)
            {
                Easyui.Redirect(PlatsUrlWriter.GetUrl());
            }

            this.Model = new
            {
                UserName = Cookies.Current["erp_username"]
            };

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var user = new Yahv.Plats.Services.LoginUser
            {
                Password = Request.Form["password"].Trim(),
                UserName = Request.Form["UserName"].Trim()
            };

            user.LoginSuccess += User_LoginSuccess;
            user.LoginFailed += User_LoginFailed;
            user.LoginClosed += User_LoginClosed;
            user.RoleErrorClosed += User_RoleErrorClosed;
            user.PasswordExpire += User_PasswordExpire;
            user.Login();
        }

        private void User_PasswordExpire(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            var admin = sender as Yahv.Plats.Services.Models.AdminRoll;
            string url = $"/Changes/ModifyPassword.aspx?userName={admin.UserName}";
            Easyui.Redirect("操作提示", "密码已经过期，请您修改密码!", url, Sign.Warning);
        }

        private void User_RoleErrorClosed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            Easyui.Redirect(this.hRoleError_url.Value);
        }

        private void User_LoginFailed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            this.Model = new
            {
                UserName = Request.Form["UserName"].Trim()
            };
            Easyui.Alert("提示", "用户名密码错误！", Sign.Warning);
        }

        private void User_LoginClosed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            Easyui.Alert("提示", "该账号已停用！", Sign.Warning);
        }

        private void User_LoginSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            var admin = sender as Yahv.Plats.Services.Models.AdminRoll;
            var url = Request.Url;
            Cookies.Current["erp_username"] = admin.UserName.Trim();
            //多用户登录
            if (Request["isMultLogin"] == "1")
            {
               

                //http://erp8.ic360.cn/Login.aspx

                string target;
                //用替换的办法做就是因为拿不准未来的方法！！！！！！！！！
                if (PlatsUrlWriter.IsResolution)
                {//获取地址中的UserName：登录名称
                    Regex regex = new Regex(@"://(.*?)\.(.*?)\.(.*?)\.(.*?)\/", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    var userName = regex.Match(url.OriginalString).Groups[1].Value;
                    target = url.OriginalString.Replace($"://admin-{userName}.", $"://admin-{admin.UserName}.");
                }
                else
                {
                    target = url.OriginalString.Replace("://", $"://admin-{admin.UserName}.");
                }

                string token = Yahv.Utils.Http.iSession.Current["ltoken"] as string;
                Uri uri = new Uri(new Uri(target), "/MultLoginers.aspx?token=" + token);
                Yahv.Plats.Services.LoginUser.SetToken("");

              
                Easyui.Redirect(uri.OriginalString);
            }
            else
            {
                Easyui.Redirect("/Panels.aspx");
            }
        }
    }
}