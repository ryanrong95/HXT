using System;

namespace Needs.Web.Sso.Forms
{
    public class UserPage : Needs.Web.Forms.UserPage
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
