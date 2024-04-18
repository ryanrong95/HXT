using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Yahv.PsWms.DappApi
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private static void Current_SqlError(object sender, EventArgs e)
        {
            Services.Logs.Log(sender.ToString(),new Exception());
        }
        protected void Application_Start()
        {

            /*
            逻辑流程：
            1.取出出库的 等待收货 的是快递的订单；
            2.判断是7天后自动确认收货更改状态为已完成，并更新客户端订单状态为已完成
            */

            Services.ConfirmReceipt.Current.SqlError += Current_SqlError;
            Services.ConfirmReceipt.Current.Start();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
