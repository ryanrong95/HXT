using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.ClientDetails
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.IsAssignSale = Erp.Current.Role.ID == Underly.FixedRole.ClientManager.GetFixedID() || Erp.Current.IsSuper || Erp.Current.Role.ID == Underly.FixedRole.SaleManager.GetFixedID();
                var clientid = Request.QueryString["id"];
                this.Model.Entity = new YaHv.Csrm.Services.Views.Rolls.ClientsRoll()[clientid];

            }
        }
    }
}