using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nContacts
{
    public partial class Details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string clientid = Request.QueryString["clientid"];
                string supplierid = Request.QueryString["supplierid"];
                string contactid = Request.QueryString["contactid"];
                this.Model.Entity = Erp.Current.Whs.WsClients[clientid].nSuppliers[supplierid].nContacts[contactid];
            }
        }
    }
}