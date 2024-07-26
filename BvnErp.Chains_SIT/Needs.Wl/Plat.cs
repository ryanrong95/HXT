using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Linq;

namespace Needs.Wl.Admin.Plat
{
    /// <summary>
    /// 华芯通物流管理端入口
    /// </summary>
    public sealed partial class AdminPlat : Erp.Generic.Startor<Models.IAdminLocale>
    {
        static AdminPlat()
        {

        }

        new static string Token
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["IsChains"]))
                {
                    return Cookies.Current[SettingsManager<Settings.Models.IYahvAdminSettings>.Current.LoginCookieName];
                }
                else
                {
                    return Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName];
                }
            }
        }

        new static public Models.IAdminLocale Current
        {
            get
            {
                if (Cookies.Supported)
                {
                    string token = Token;
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return null;
                    }

                    return GetAdmin(token);
                }
                else
                {
                    return null;
                }
            }
        }

        static public void Logout()
        {
            Cookies.Current[SettingsManager<IAdminSettings>.Current.LoginCookieName] = "";
        }

        new static Models.IAdminLocale GetAdmin(string token)
        {
            using (var view = new Views.YahvAdminsToken(token))
            {
                return view.SingleOrDefault();
            }
            //if (!bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsChains"]))
            //{
            //    using (var view = new Views.YahvAdminsToken(token))
            //    {
            //        return view.SingleOrDefault();
            //    }
            //}
            //else
            //{
            //    using (var view = new Views.AdminsView(token))
            //    {
            //        return view.SingleOrDefault();
            //    }
            //}
        }
    }
}
