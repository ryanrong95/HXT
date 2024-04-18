using Needs.Settings;
using Needs.Settings.Models;
using Needs.Utils.Http;
using Needs.Web.Models;
using Needs.Web.Views;
using System.Configuration;
using System.Linq;

namespace Needs.Web.Sso.Forms
{
    public class ErpPage : UserPage
    {
        protected int PageSize = 20;
        protected override bool Authenticate()
        {
            if(!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IsChains"]))
            {
                string token = Cookies.Current[SettingsManager<IYahvAdminSettings>.Current.LoginCookieName];
                using (YahvAdminsToken tokens = new YahvAdminsToken(token))
                {
                    return tokens.SingleOrDefault() != null;
                }
            }
            using (AdminsToken tokens = new AdminsToken(Cookies.Current[Admin.CookieName]))
            {
                return tokens.SingleOrDefault() != null;
            }
        }

        protected override void OnDenied()
        {
            Alert("用户身份验证失败", "/", false, true);
        }

        protected override void OnSucess()
        {
        }
    }
}
