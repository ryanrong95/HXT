using Needs.Wl.Logs.Services;
using System.Web.Mvc;

namespace Needs.Wl.Web.Mvc
{
    /// <summary>
    /// 自定义异常处理
    /// TODO:完成系统异常页面的开发
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