using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Linq;

namespace Needs.Erp
{
    public sealed class ErpPlot : Generic.Startor<Needs.Erp.Models.IAdminLocale>
    {
        static ErpPlot()
        {

        }

        new static public Needs.Erp.Models.IAdminLocale Current
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
            using (var view = new Views.AdminsView(token))
            {
                return view.SingleOrDefault();
            }
        }

        static public void ChangePassword(string newPassword, string oldPassword)
        {

        }


    }
}
