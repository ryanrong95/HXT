using System;
using Yahv.Plats.Services;
using Yahv.Utils;
using Yahv.Utils.Extends;
using Yahv.Utils.Http;
using Yahv.Web.Forms;

namespace WebApp
{
    public partial class Sso : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Login();
            }
        }

        /// <summary>
        /// 根据大赢家编码登录
        /// </summary>
        public void Login()
        {
            var code = Request.QueryString["code"];

            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var user = new Yahv.Plats.Services.LoginUser();
                    user.LoginSuccess += User_LoginSuccess;

                    string adminId = string.Empty;
                    var data = ZEncyptDES.DESDecrypt(code.Base64Decode());
                    user.Login(data, out adminId);

                    Yahv.Oplogs.Oplog(adminId, Request.Url.Host.ToString(),
                        nameof(Yahv.Systematic.Erm), "大赢家登录", $"大赢家ID [{data}]", $"{code}");
                }
            }
            catch (Exception ex)
            {
                Yahv.Oplogs.Oplog("Npc-Robot", Request.Url.Host.ToString(),
                    nameof(Yahv.Systematic.Erm), "大赢家登录", $"编码 [{code}]", $"{ex.Message}");
            }
        }

        private void User_LoginSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            try
            {
                Response.Redirect("/Panels.aspx", false);
            }
            catch
            {
            }
        }
    }
}