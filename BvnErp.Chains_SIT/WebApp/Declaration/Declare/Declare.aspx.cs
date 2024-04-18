using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class Declare : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];

            string Source = Request.QueryString["Source"];
            this.Model.ID = id ?? "";
            this.Model.Source = Source ?? "Edit";
        }
    }
}