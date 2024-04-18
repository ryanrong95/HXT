using System;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace WebApp
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Yahv.Erp.Logout();
        }
    }
}