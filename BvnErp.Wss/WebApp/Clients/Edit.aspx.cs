using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Clients
{
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model = Needs.Erp.ErpPlot.Current.Websites.MyClients[Request.QueryString["id"]];
        }
    }
}