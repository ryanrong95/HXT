using Needs.Utils.Http;
using Needs.Web.Models;
using Needs.Web.Mvc;
using Needs.Web.Views;
using System;
using System.Linq;

namespace Needs.Web.Sso.Mvc
{
    [Obsolete("前后端已经分离，随时准备废弃")]
    public class ErpControler : UserController
    {
        protected override bool Authenticate()
        {
            using (AdminsToken tokens = new AdminsToken(Cookies.Current[Admin.CookieName]))
            {
                return tokens.SingleOrDefault() != null;
            }
        }

        protected override void OnDenied()
        {
            throw new NotImplementedException();
        }

        protected override void OnSucess()
        {
            //throw new NotImplementedException();
        }
    }
}
