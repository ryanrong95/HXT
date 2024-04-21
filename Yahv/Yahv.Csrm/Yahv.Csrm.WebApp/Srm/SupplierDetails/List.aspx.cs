using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.SupplierDetails
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                this.Model = new SuppliersRoll()[id];
            }
        }
    }
}