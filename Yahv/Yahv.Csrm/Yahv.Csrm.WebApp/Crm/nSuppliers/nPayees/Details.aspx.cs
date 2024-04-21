using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nPayees
{
    public partial class Details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string clientid = Request.QueryString["clientid"];
                string supplierid = Request.QueryString["supplierid"];
                string payeeid = Request.QueryString["payeeid"];
                this.Model.Entity = Erp.Current.Whs.WsClients[clientid].nSuppliers[supplierid].nPayees[payeeid];
            }
        }
    }
}