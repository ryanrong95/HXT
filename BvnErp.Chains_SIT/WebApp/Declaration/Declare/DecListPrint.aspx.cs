using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecListPrint : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Print()
        {
            string DeclarationID = Request.Form["ID"];
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];
            CheckDocuments doc = new CheckDocuments(DecHead);
            string jsonResult = doc.Json();

            Response.Write(new { result = true, info = jsonResult }.Json());
        }
    }
}