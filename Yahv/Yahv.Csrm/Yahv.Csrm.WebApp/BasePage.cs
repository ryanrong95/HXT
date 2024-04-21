using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.Csrm.WebApp
{
    public class BasePage : Yahv.Web.Forms.Sso.ErpPage
    {
        protected override bool Authenticate()
        {
            return Yahv.Erp.Current == null ? false : true;
        }

        protected override void OnDenied()
        {
            Response.Redirect("/Login.aspx");
        }

        protected override void OnSucess()
        {
            
        }
    }
}