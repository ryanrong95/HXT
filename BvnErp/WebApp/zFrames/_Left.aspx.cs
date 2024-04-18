
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.zFrames
{
    public partial class _Left : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model = Needs.Erp.ErpPlot.Current.Plots.MyMenus.ToArray();
            }
        }
    }
}