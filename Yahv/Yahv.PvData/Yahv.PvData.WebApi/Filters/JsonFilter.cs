using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.Http.Filters;

namespace Yahv.PvData.WebApi.Filters
{
    public class JsonPActionFilterAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        //{
        //    string callback = HttpContext.Current.Request.QueryString["callback"];
        //    if (HttpContext.Current.Request.HttpMethod == "GET" && !string.IsNullOrWhiteSpace(callback))
        //    {
        //        if (actionExecutedContext.Response != null)
        //        {
        //            object content = actionExecutedContext.Response.Content;
        //            HttpContext.Current.Response.Write($"{callback}({Newtonsoft.Json.JsonConvert.SerializeObject(content.GetType().GetProperty("Value").GetValue(content), Newtonsoft.Json.Formatting.None)})");
        //            HttpContext.Current.Response.End();
        //        }
        //    }
        //    else
        //    {
        //        base.OnActionExecuted(actionExecutedContext);
        //    }
        //}
    }

}