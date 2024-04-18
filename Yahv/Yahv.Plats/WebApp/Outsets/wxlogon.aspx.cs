using System;
using System.Configuration;
using System.Web;

namespace WebApp.Outsets
{
    public partial class wxlogon : System.Web.UI.Page
    {
        public static string WXLogonCodes
        {
            get
            {
                return HttpContext.Current.Session[nameof(WXLogonCodes)] as string;
            }
            set
            {
                HttpContext.Current.Session[nameof(WXLogonCodes)] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var appid = ConfigurationManager.AppSettings["weixinAppID"];
            
            var CallBackURI = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["weixinCallBackURI"]);

            var context = new QConnectSDK.Context.QzoneContext();
            string state = Guid.NewGuid().ToString().Replace("-", "");
            var authenticationUrl = "https://open.weixin.qq.com/connect/qrconnect?appid=" + appid + "&redirect_uri=" + CallBackURI + "&response_type=code&scope=snsapi_login&state=" + state + "#wechat_redirect";
            WXLogonCodes = state;
            HttpContext.Current.Response.Redirect(authenticationUrl);
        }
    }
}