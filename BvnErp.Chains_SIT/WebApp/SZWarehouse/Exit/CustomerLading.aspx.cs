using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Exit
{
    public partial class CustomerLading : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("QuickComplete.aspx?RedirectFrom=CustomerLading");
        }


    }
}