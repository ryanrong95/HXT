using Needs.Wl.Logs.Services;
using System.Web.Mvc;

namespace Needs.Wl.Web.WeChat
{
    /// <summary>
    /// 自定义异常处理
    /// </summary>
    public class UserHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.Exception.Log();
            base.OnException(filterContext);
        }
    }
}