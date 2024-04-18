using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Express100
{
    public partial class EMSExpressTest : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void EmsTestAction() {

            var ss = new EmsApiHelper().EmsXmlGenerate(new EmsRequestModel());

            Response.Write((new { success = true, message = ss }).Json());
        }

    }
}