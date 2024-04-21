using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Csrm.WebApp.Crm.WsClientsDetails
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                string companyid = Request.QueryString["CompanyID"];
                var wsclient = Erp.Current.Whs.WsClients[companyid, clientid];
                this.Model.Entity= wsclient;
                this.Model.Standard = wsclient.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase);
                this.Model.IsPersonal = wsclient.StorageType == Underly.WsIdentity.Personal||wsclient.StorageType==Underly.WsIdentity.Mainland;
            }
        }
    }
}