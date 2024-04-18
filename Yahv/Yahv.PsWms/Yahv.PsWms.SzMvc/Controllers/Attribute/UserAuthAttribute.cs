using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public class LoginCheckAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否需要验证
        /// </summary>
        public bool IsNeedCheck { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsNeedCheck)
            {
                if (Yahv.PsWms.SzMvc.SiteCoreInfo.Current == null)
                {
                    //需要检查并且未登录
                    filterContext.Result = new RedirectResult("/Home/Login");
                }
            }
            else if (IsNeedCheck == false
                && Yahv.PsWms.SzMvc.SiteCoreInfo.Current != null
                && filterContext.HttpContext.Request.Url.AbsolutePath == "/Home/Login")
            {
                //不要检查, 并且已经登录, 并且访问登录页面
                filterContext.Result = new RedirectResult("/Home/Index");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}