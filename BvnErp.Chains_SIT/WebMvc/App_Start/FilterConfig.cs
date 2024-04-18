using System.Web;
using System.Web.Mvc;
using Needs.Wl.Web.Mvc;

namespace WebMvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //设置全局授权过滤器
            filters.Add(new HandleErrorAttribute());
        }
    }
}
