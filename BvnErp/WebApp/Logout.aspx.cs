using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp
{
    public partial class Logout : Needs.Web.Forms.ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Needs.Erp.ErpPlot.Logout();
        }
    }
}