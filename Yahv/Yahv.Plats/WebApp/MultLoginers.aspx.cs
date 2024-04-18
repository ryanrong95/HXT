using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Utils.Http;
using Yahv.Web.Forms;

namespace WebApp
{
    /// <summary>
    /// 点击多用户登录，调用的地点
    /// </summary>
    public partial class MultLoginers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Yahv.Plats.Services.LoginUser.SetToken(Request.QueryString["token"]);
        }
    }
}