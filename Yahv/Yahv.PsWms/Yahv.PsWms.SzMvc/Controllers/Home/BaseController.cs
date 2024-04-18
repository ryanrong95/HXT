using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.OfficialWebsite = System.Configuration.ConfigurationManager.AppSettings["OfficialWebsite"];

            //是否有查看详情跳转
            string para = Request.Form["para"];
            if (!string.IsNullOrEmpty(para))
            {
                var paraArr = JsonConvert.DeserializeObject<string[]>(para);
                ViewBag.LastUrl = paraArr[1];
                ViewBag.id = paraArr[0];
            }

            //当前地址
            ViewBag.CurrentUrl = @"/" + Convert.ToString(RouteData.Values["controller"]) + @"/" + Convert.ToString(RouteData.Values["action"]);

            //用户基本信息
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            if (siteuser != null)
            {
                ViewBag.SiteuserName = siteuser.Username;

                var clients = siteuser.Clients;
                if (clients != null && clients.Length > 0)
                {
                    ViewBag.ClientName = clients[0].ClientName;
                }
            }
        }
    }
}