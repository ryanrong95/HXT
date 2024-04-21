using System.Web.Mvc;
using NLog;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Filter
{
    /// <summary>
    /// 日志过滤器
    /// </summary>
    public class LogFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string args = string.Empty;
            foreach (var parameter in filterContext.ActionParameters)
            {
                args += $" {parameter.Key}: {parameter.Value.Json()} ";
            }

            LogManager.GetLogger(GetLoggerName(filterContext)).Info($"{filterContext.ActionDescriptor.ActionName}({args})");
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            string result = string.Empty;

            if (filterContext.Result is JsonResult)
            {
                var json = (JsonResult)filterContext.Result;
                result = $"{json.Data.Json()}";
            }

            if (filterContext.Result is JsonpResult )
            {
                var json = (JsonResult)filterContext.Result;
                result = $"{json.Data.Json()}";
            }

            if (!string.IsNullOrEmpty(result))
            {
                LogManager.GetLogger(GetLoggerName(filterContext)).Info(result);
            }
        }

        /// <summary>
        /// 异常过滤
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            NLog.LogManager.GetLogger(GetLoggerName(filterContext)).Error(filterContext.Exception);
        }

        #region 获取logger名称
        /// <summary>
        /// 获取logger名称
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private string GetLoggerName(ControllerContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"]?.ToString();
            string action = filterContext.RouteData.Values["action"]?.ToString();

            return $"{controller}Controller.{action}";
        }
        #endregion
    }
}