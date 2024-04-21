using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp
{
    public partial class Home : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           // if (Request.QueryString.Keys.Count == 0)
           // {
           //     Easyui.AutoRedirect("测试", Request.Url.AbsolutePath + "?ID=1",
           //Web.Controls.Easyui.AutoSign.Success);
           // }
        }
    }
}