using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.WsConsignees
{
    public partial class Details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
                this.Model.Entity = wsclient.Consignees[Request.QueryString["id"]];
            }
        }
    }
}