using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Yahv.Utils.Serializers;

namespace Yahv.FileServices
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //new Thread(new ThreadStart(delegate {
            //    while (true)
            //    {
                    
            //        Models.UploadConfig.Configs = File.ReadAllText(HttpContext.Current.Server.MapPath("/App_Data/Config.json")).JsonTo<Models.Config[]>();
            //        Thread.Sleep(5 * 1000);
            //    }
            //})).Start();
        }
    }
}
