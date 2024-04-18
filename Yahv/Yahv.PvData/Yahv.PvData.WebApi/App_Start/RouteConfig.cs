using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Yahv.PvData.WebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                  name: "ExchangeRates1",
                  url: "ExchangeRates/{action}/{from}/{to}/{price}/{type}/",
                  defaults: new
                  {
                      action = "Index",
                      from = UrlParameter.Optional,
                      to = UrlParameter.Optional,
                      price = UrlParameter.Optional,
                      type = UrlParameter.Optional,
                  });
        }
    }
}
