using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public class DebugAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string isDebug = ConfigurationManager.AppSettings["IsDebug"];

            if (!string.IsNullOrEmpty(isDebug) && isDebug == "1")
            {
                string[] controllerStrs = filterContext.Controller.ToString().Split('.');
                string controllerName = controllerStrs[controllerStrs.Length - 1].Replace("Controller", "");
                string actionName = filterContext.ActionDescriptor.ActionName;
                string absolutePath = HttpContext.Current.Server.MapPath(Path.Combine(@"\Content\json", controllerName, actionName + ".json"));
                string jsonStr = File.ReadAllText(absolutePath, Encoding.UTF8);

                var request = HttpContext.Current.Request;
                if (!string.IsNullOrEmpty(request.Form["page"]) && !string.IsNullOrEmpty(request.Form["rows"]))
                {
                    int page = int.Parse(request.Form["page"]);
                    int rows = int.Parse(request.Form["rows"]);

                    var allDataObj = JsonConvert.DeserializeObject<List<object>>(jsonStr);
                    var partDataObj = allDataObj.Skip(rows * (page - 1)).Take(rows);

                    var resultObj = new
                    {
                        type = "success",
                        msg = "",
                        data = new
                        {
                            list = partDataObj,
                            total = allDataObj.Count(),
                        }
                    };

                    jsonStr = JsonConvert.SerializeObject(resultObj);
                }

                filterContext.Result = new DebugJsonResult(jsonStr);
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class DebugJsonResult : JsonResult
    {
        public string JsonStr { get; private set; }

        public DebugJsonResult(string jsonStr)
        {
            JsonStr = jsonStr;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.Output.Write(JsonStr);
        }
    }
}