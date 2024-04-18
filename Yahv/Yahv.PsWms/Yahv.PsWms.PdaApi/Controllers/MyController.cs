using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.PdaApi.Controllers
{
    public class MyController : Controller
    {
        public ActionResult Enter()
        {
            
            object obj1 = new
            {
                success = 200,
                result = new
                {
                    value = 1,
                    text = "台 (001)"
                }
            };

            var str = JsonConvert.SerializeObject(obj1);

            return new MyJsonResult(str);
        }
    }

    public class MyJsonResult : ActionResult
    {
        public string JsonData { get; private set; }

        public MyJsonResult(object data)
        {
            JsonData = data.ToString();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Output.Write(JsonData);
        }
    }
}