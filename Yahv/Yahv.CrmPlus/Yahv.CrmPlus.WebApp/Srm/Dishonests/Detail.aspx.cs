using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.Dishonests
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                this.Model.Entity = new Service.Views.Rolls.SupplierDishonestsRoll()[id];
            }
        }
    }
}