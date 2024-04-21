using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.ClientDetails
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["id"];
                this.Model.Entity = new YaHv.Csrm.Services.Views.Rolls.TradingClientsRoll()[clientid];
                this.Model.IsSale = Yahv.Erp.Current.Role.ID == FixedRole.Sale.GetFixedID();
            }
        }
    }
}