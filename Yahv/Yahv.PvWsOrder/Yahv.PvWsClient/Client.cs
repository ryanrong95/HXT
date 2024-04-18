using Layers.Data.Sqls.PvbErm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.PvWsClient.Model;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Utils.Http;

namespace Yahv
{
    abstract public class ClientBase
    {
        /// <summary>
        /// 通过Token获取当前会员
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        protected ClientUser GetByToken(string Token)
        {
            using (var view = new PvWsClient.Views.UserToken(Token))
            {
                var entity = view.SingleOrDefault();
                return entity;
            }
        }

        /// <summary>
        /// 通过 ID 获取 Admin 数据
        /// </summary>
        /// <returns>Admin 实例</returns>
        protected ClientUser GetByID(string id)
        {
            using (var view = new PvWsClient.Views.UsersAlls())
            {
                var entity = view[id];
                return entity;
            }
        }

        /// <summary>
        /// 通过UserName获取User
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        protected ClientUser GetByUserID(string UserID)
        {
            string ip = ClientIP.GetWebClientIp();
            using (var view = new PvWsClient.Views.UserTopView())
            {
                var entity = view.FirstOrDefault(item => item.ID == UserID && item.IP == ip);
                return entity;
            }
        }
    }


    public sealed class Client : ClientBase
    {
        static Client client;
        static object locker = new object();

        static public ClientUser Current
        {
            get
            {
                var mobile = HttpContext.Current.Request.Headers.Get("mobile");
                var tokenMobile = HttpContext.Current.Request.Headers.Get("token");

                if (!string.IsNullOrEmpty(mobile) && mobile == "1")
                {
                    // 是手机端
                    var userMobile = GetCurrentUserMobile(tokenMobile);
                    return userMobile;
                }

                // 下面是原来的网页端获取当前用户

                if (!Cookies.Supported)
                {
                    return null;
                }
                if (client == null)
                {
                    lock (locker)
                    {
                        if (client == null)
                        {
                            client = new Client();
                        }
                    }
                }

                //获取当前token,是否记住密码
                var token = Cookies.Current[PvWsClient.Setting.SettingsManager<IUserSetting>.Current.LoginCookieName];
                if(string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(Yahv.PvWsOrder.Services.PvClientConfig.Domain))
                {
                    token = Cookies.Domain[Yahv.PvWsOrder.Services.PvClientConfig.Domain][PvWsClient.Setting.SettingsManager<IUserSetting>.Current.WebCookieName];
                }
                var isremeber = Cookies.Current[PvWsClient.Setting.SettingsManager<IUserSetting>.Current.LoginRemeberName];
                ClientUser user = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    //从Session中获取登录会员
                    user = iSession.Current[token] as ClientUser;
                    if (user == null)
                    {
                        lock (locker)
                        {
                            if (user == null)
                            {
                                user = client.GetByToken(token);
                                if(user != null)
                                {
                                    user.IsRemeber = bool.Parse(isremeber ?? "False");
                                    user.Token = token;
                                    iSession.Current[token] = user;
                                }
                            }
                        }
                    }
                }
                return user;
            }
        }

        private static ClientUser GetCurrentUserMobile(string token)
        {
            ClientUser user = null;

            if (client == null)
            {
                lock (locker)
                {
                    if (client == null)
                    {
                        client = new Client();
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                lock (locker)
                {
                    if (user == null)
                    {
                        user = client.GetByToken(token);
                        if (user != null)
                        {
                            user.IsRemeber = false;
                            user.Token = token;
                        }
                    }
                }
            }

            return user;
        }

    }
}
