using Needs.Utils.Http;
using Needs.Web.Models;
using System;

namespace Needs.Web.Sso.Mvc
{
    [Obsolete("前后端已经分离，随时准备废弃")]
    public class UserControler : Needs.Web.Mvc.UserController
    {
        protected override bool Authenticate()
        {
            throw new NotImplementedException();
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
