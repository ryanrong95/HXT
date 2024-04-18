using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Models;
using Yahv.Utils.Http;

namespace Yahv.PsWms.SzMvc
{
    public sealed class SiteCoreInfo
    {
        static SiteCoreInfo client;
        static object locker = new object();

        static public Siteuser Current
        {
            get
            {
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
                            client = new SiteCoreInfo();
                        }
                    }
                }

                string domain = ConfigurationManager.AppSettings["Domain"];

                //获取当前token,是否记住密码
                var token = Cookies.Current[SettingsManager<IUserSetting>.Current.LoginCookieName];
                if (string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(domain))
                {
                    token = Cookies.Domain[domain][SettingsManager<IUserSetting>.Current.WebCookieName];
                }
                var isremeber = Cookies.Current[SettingsManager<IUserSetting>.Current.LoginRemeberName];
                Siteuser user = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    //从Session中获取登录会员
                    user = iSession.Current[token] as Siteuser;
                    if (user == null)
                    {
                        lock (locker)
                        {
                            if (user == null)
                            {
                                user = client.GetByToken(token);
                                if (user != null)
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

        /// <summary>
        /// 通过Token获取当前会员
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Siteuser GetByToken(string token)
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var siteusers = repository.ReadTable<Layers.Data.Sqls.PsOrder.Siteusers>();
                var userTokens = repository.ReadTable<Layers.Data.Sqls.PsOrder.UserTokens>();
                var clients = repository.ReadTable<Layers.Data.Sqls.PsOrder.Clients>();

                var theSiteuser = (from siteuser in siteusers
                                   join userToken in userTokens
                                     on siteuser.ID equals userToken.UserID
                                   where userToken.Token == token
                                   select new Siteuser
                                   {
                                       SiteuserID = siteuser.ID,
                                       Username = siteuser.Username,
                                       //Password = siteuser.Password,
                                       Token = userToken.Token,
                                   }).FirstOrDefault();

                theSiteuser.Clients = clients.Where(t => t.SiteuserID == theSiteuser.SiteuserID)
                                        .Select(item => new Siteuser.Client
                                        {
                                            ClientID = item.ID,
                                            ClientName = item.Name,
                                            TrackerID = item.TrackerID,
                                        }).ToArray();

                return theSiteuser;
            }
        }
    }
}
