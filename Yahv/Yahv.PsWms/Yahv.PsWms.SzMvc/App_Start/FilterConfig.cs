using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Controllers;

namespace Yahv.PsWms.SzMvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginCheckAttribute() { IsNeedCheck = true });
        }
    }
}
