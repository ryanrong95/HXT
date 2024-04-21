using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers
{
    public partial class Details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string nSupplierID = Request.QueryString["id"];
                string ClientID = Request.QueryString["clientid"];
                this.Model.Entity = Erp.Current.Whs.WsClients[ClientID].nSuppliers[nSupplierID];
            }
        }
    }
}