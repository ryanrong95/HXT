using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppTester
{
    public partial class CookiesTester : System.Web.UI.Page
    {

        protected bool isOpen;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Status
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            isOpen = true;

            HttpCookie cookie = new HttpCookie("MyCook");//初使化并设置Cookie的名称
            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, 0, 1, 0, 0);//过期时间为1分钟
            cookie.Expires = dt.Add(ts);//设置过期时间

            cookie.Value = "userid_value:jim," + DateTime.Now.Ticks;
            cookie.Secure = Request.Url.OriginalString.StartsWith("https");
            cookie.SameSite = SameSiteMode.None;

            Response.AppendCookie(cookie);
        }
    }
}