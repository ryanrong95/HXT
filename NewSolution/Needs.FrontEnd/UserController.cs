using System.Web.Mvc;

namespace Needs.FrontEnd
{
    abstract public class UserController : ClientController
    {
        public UserController()
        {

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Cookies.Current["toekn"] = "";
            //Cookies.Domain[".b1b.com"][DateTime.Now.AddYears(1)]["toekn"] = "";
            //Cookies.Current[DateTime.Now.AddYears(1)]["toekn"] = "1212";

            if (this.Authenticate())
            {
                this.OnDenied();
            }
            else

            {
                this.OnSucess();
            }
        }

        abstract protected bool Authenticate();
        abstract protected void OnSucess();
        abstract protected void OnDenied();
    }
}