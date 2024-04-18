using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Yahv.Web.Faces;

namespace Yahv.PvData.WebApi
{
    public class MvcApplication : Globals
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected override void Globals_BeginRequest_Request(object sender, EventArgs e)
        {
            base.Globals_BeginRequest_Request(sender, e);
        }
    }
}
