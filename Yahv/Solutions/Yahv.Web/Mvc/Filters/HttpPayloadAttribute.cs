using System.Reflection;
using System.Web.Mvc;

namespace Yahv.Web.Mvc.Filters
{
    /// <summary>
    /// Payload 过滤器
    /// </summary>
    public class HttpPayloadAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// 请求验证
        /// </summary>
        /// <param name="controllerContext">控制器上下文</param>
        /// <param name="methodInfo">action 信息</param>
        /// <returns></returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request.HttpMethod == "POST";
                //&& controllerContext.HttpContext.Request.ContentType.Contains("application/json");
        }
    }
}
