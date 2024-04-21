using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.SupplierID = Request.QueryString["id"];
                this.Model.EnterpriseID = Request.QueryString["enterpriseid"];
            }
        }
    }
}