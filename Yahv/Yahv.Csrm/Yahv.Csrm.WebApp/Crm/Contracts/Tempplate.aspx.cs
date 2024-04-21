using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.Contracts
{
    public partial class Tempplate : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["clientid"];
                string companyid = Request.QueryString["companyid"];
                var wsclient = Erp.Current.Whs.WsClients[companyid, clientid];
                this.Model.Infor = wsclient.ContractDic();
            }
        }
    }
}