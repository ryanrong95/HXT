using Needs.Wl.User.Plat;
using System.Web.Mvc;

namespace Needs.Wl.Web.Mvc
{
    /// <summary>
    /// 会员身份验证
    /// </summary>
    public class UserActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否需要身份验证
        /// </summary>
        public bool UserAuthorize = true;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserAuthorize)
            {
                if (UserPlat.Current == null)
                {
                    filterContext.Result = new RedirectResult("/Home/Login");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}