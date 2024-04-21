using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Contacts
{
    public partial class Detail : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Contact = new YaHv.Csrm.Services.Views.Rolls.ContactsRoll()[Request.QueryString["id"]];
            }

        }
    }
}