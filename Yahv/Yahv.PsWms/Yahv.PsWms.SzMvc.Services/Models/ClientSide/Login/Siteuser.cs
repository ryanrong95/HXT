using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    public class Siteuser
    {
        /// <summary>
        /// SiteuserID
        /// </summary>
        public string SiteuserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否记住我
        /// </summary>
        public bool IsRemeber { get; set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        public string Token { get; internal set; }

        /// <summary>
        /// Clients
        /// </summary>
        public Client[] Clients { get; set; }

        public class Client
        {
            /// <summary>
            /// ClientID
            /// </summary>
            public string ClientID { get; set; }

            /// <summary>
            /// ClientName
            /// </summary>
            public string ClientName { get; set; }

            /// <summary>
            /// TrackerID
            /// </summary>
            public string TrackerID { get; set; }
        }

        private string _theClientID { get; set; }

        /// <summary>
        /// 当前 ClientID
        /// </summary>
        public string TheClientID
        {
            get
            {
                _theClientID = string.Empty;
                if (this.Clients != null && this.Clients.Length > 0)
                {
                    _theClientID = this.Clients[0].ClientID;
                }
                return _theClientID;
            }
            set { _theClientID = value; }
        }

        private string _theTrackerID { get; set; }

        public string TheTrackerID
        {
            get
            {
                _theTrackerID = string.Empty;
                if (this.Clients != null && this.Clients.Length > 0)
                {
                    _theTrackerID = this.Clients[0].TrackerID;
                }
                return _theTrackerID;
            }
            set { _theTrackerID = value; }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            string password = this.Password.StrToMD5();
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var theSiteuser = repository.ReadTable<Layers.Data.Sqls.PsOrder.Siteusers>()
                                    .Where(t => t.Username == this.Username)
                                    .Select(item => new
                                    {
                                        item.ID,
                                        item.Username,
                                        item.Password,
                                    }).FirstOrDefault();

                //是否没有当前用户
                if (theSiteuser == null || (theSiteuser.Password != password && theSiteuser.Password != this.Password))
                {
                    throw new Exception("账号或密码不正确!");
                }

                this.SiteuserID = theSiteuser.ID;
            }

            //用户名密码验证成功后
            string ip = ClientIP.GetWebClientIp();
            DateTime loginDate = DateTime.Now;
            string token = string.Concat("$", this.Username, "*", "&", loginDate.ToString(), "#").MD5("x");

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Insert(new Layers.Data.Sqls.PsOrder.UserTokens
                {
                    ID = "TOKEN" + loginDate.Ticks,
                    CreateDate = loginDate,
                    UserID = this.SiteuserID,
                    Token = token,
                    IP = ip,
                });

                try
                {
                    if (HttpContext.Current.Request.Url.Authority.Contains("localhost") == false)
                    {
                        repository.Update<Layers.Data.Sqls.PsOrder.Siteusers>(new
                        {
                            LoginDate = DateTime.Now
                        }, item => item.ID == this.SiteuserID);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            if (Cookies.Supported)
            {
                string domain = ConfigurationManager.AppSettings["Domain"];

                if (!string.IsNullOrWhiteSpace(domain))
                {
                    var cookie = Cookies.Domain[domain];
                    cookie[SettingsManager<IUserSetting>.Current.WebCookieName] = token;
                }
                Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName] = token;
                Cookies.Current[SettingsManager<IUserSetting>.Current.LoginRemeberName] = this.IsRemeber.ToString();
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public void LoginOut()
        {
            if (!Cookies.Supported)
            {
                throw new NotImplementedException("没有实现不支持cookies的浏览器的登入与退出");
            }
            string token = Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName];
            //先把Session信息清空
            iSession.Current[token] = null;

            //清空Cookie
            Cookies.Current.Append(SettingsManager<IUserSetting>.Current.LoginCookieName, "", DateTime.Now.AddDays(-1));
            if (this.IsRemeber)
            {
                Cookies.Current[SettingsManager<IUserSetting>.Current.LoginUserIDName] = this.Username + "," + this.Password;
            }
            else
            {
                Cookies.Current.Append(SettingsManager<IUserSetting>.Current.LoginRemeberName, "False", DateTime.Now.AddDays(-1));
                Cookies.Current.Append(SettingsManager<IUserSetting>.Current.LoginUserIDName, "", DateTime.Now.AddDays(-1));
            }

            string domain = ConfigurationManager.AppSettings["Domain"];

            if (!string.IsNullOrWhiteSpace(domain))
            {
                Cookies.Domain[domain].Append(SettingsManager<IUserSetting>.Current.WebCookieName, "", DateTime.Now.AddDays(-1));
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void ChangePassword(string oldPassword, string newPassword)
        {
            oldPassword = oldPassword.StrToMD5();
            newPassword = newPassword.StrToMD5();

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var theSiteuser = repository.ReadTable<Layers.Data.Sqls.PsOrder.Siteusers>()
                                .Where(t => t.ID == this.SiteuserID)
                                .Select(item => new
                                {
                                    SiteuserID = item.ID,
                                    Password = item.Password
                                }).FirstOrDefault();
                if (theSiteuser == null || theSiteuser.Password != oldPassword)
                {
                    throw new Exception("当前密码输入错误，请重新输入！");
                }

                repository.Update<Layers.Data.Sqls.PsOrder.Siteusers>(new
                {
                    Password = newPassword,
                }, item => item.ID == this.SiteuserID);
            }

            this.ClearSession();
        }

        /// <summary>
        /// 清空Session
        /// </summary>
        public void ClearSession()
        {
            string token = Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName];
            //先把Session信息清空
            iSession.Current[token] = null;
        }

    }
}
