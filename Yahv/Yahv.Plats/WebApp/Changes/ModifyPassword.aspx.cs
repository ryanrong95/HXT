using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv;
using Yahv.Models;
using Yahv.Plats.Services;
using Yahv.Plats.Services.Models;
using Yahv.Underly;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace WebApp.Changes
{
    public partial class ModifyPassword : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object Submit()
        {
            JMessage json = new JMessage() { success = true, data = "修改成功!" };

            try
            {
                new LoginUser().ModifyPassword(Request.Form["adminID"].Trim(), Request.Form["passwordOld"], Request.Form["passwordNew"]);
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = ex.Message;
            }

            return json;
        }
    }
}