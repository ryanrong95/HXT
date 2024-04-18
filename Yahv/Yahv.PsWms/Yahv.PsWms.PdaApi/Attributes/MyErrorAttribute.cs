using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;

namespace Yahv.PsWms.PdaApi.Attributes
{
    public class MyErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            var msg = Newtonsoft.Json.JsonConvert.SerializeObject(new JMessage
            {
                success = false,
                code = 400,
                data = filterContext.Exception.Message,
            });
            filterContext.HttpContext.Response.Write(msg);
        }
    }
}