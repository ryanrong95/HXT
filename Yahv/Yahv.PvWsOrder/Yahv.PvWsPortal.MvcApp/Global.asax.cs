using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Yahv.PvWsPortal.MvcApp
{
    public class MvcApplication : System.Web.HttpApplication
    {

        public MvcApplication()
        {
            this.BeginRequest += MvcApplication_BeginRequest;
        }

        private void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            if (Request.Url.AbsolutePath == "/")
            {
                string html = @"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <title></title>
    <meta charset=""utf-8"" />
</head>
<body>
    <script>
        location.replace('Index.html');
    </script>
</body>
</html>";
                Response.ClearContent();
                Response.Write(html);
                Response.End();
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
