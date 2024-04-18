using System;
using System.Web.Mvc;

namespace Needs.Web.Mvc
{
    //OnActionExecuting是Action执行前的操作
    //OnActionExecuted则是Action执行后的操作
    //OnResultExecuting是解析ActionResult前执行
    //OnResultExecuted是解析ActionResult后执行
    [Obsolete("前后端已经分离，随时准备废弃")]
    public class UserFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

    }
}
