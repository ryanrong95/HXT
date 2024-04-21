using System.Web;
using System.Web.Mvc;
using Yahv.Finance.WebApi.Filter;

namespace Yahv.Finance.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogFilterAttribute());
        }
    }
}
