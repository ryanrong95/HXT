using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyClients
{
    public partial class Tabs : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                string clientid = Request.QueryString["ClientID"];
                bool isclient = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.IsMyclient(clientid);
                this.hidIsClient.Value = isclient.ToString();
            }
        }
    }
}