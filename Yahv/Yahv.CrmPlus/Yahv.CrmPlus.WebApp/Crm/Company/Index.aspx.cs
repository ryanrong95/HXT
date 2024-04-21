using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Company
{
    public partial class Index : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                this.Model = new CompaniesRoll()[id];
            }
        }
    }
}