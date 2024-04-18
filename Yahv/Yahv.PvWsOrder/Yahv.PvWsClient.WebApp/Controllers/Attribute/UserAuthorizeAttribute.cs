using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PvWsClient.WebApp.Controllers
{
    public class UserAuthorizeAttribute: ActionFilterAttribute
    {
        /// <summary>
        /// 是否需要身份验证
        /// </summary>
        public bool UserAuthorize = true;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.UserAuthorize)
            {
                if (Yahv.Client.Current == null)
                {

                    filterContext.Result = new RedirectResult("/Home/Login");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}