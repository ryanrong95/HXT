using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Csrm.WebApp.Crm.WsClientsDetails
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                this.Model.Entity = Erp.Current.Whs.WsClients[clientid];

            }
        }
    }
}