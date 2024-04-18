using System.Web.Mvc;

namespace Needs.Wl.Web.WeChat
{
    public class UserAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否需要身份验证
        /// </summary>
        public bool UserAuthorize = true;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserAuthorize)
            {
                if (Needs.Wl.User.Plat.WeChatPlat.Current == null)
                {
                    filterContext.Result = new RedirectResult("/Home/Login");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}