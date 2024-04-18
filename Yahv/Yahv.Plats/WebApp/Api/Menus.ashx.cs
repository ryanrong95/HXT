using System.Web;

namespace WebApp.Api
{
    /// <summary>
    /// MenusHandler 的摘要说明
    /// </summary>
    public class Menus : Yahv.Web.Handlers.JsonHandler
    {
        override protected void OnProcessRequest(HttpContext context)
        {
            string json = Yahv.Erp.Current.Plat.MyMenus.Json();
            context.Response.Write(json);
        }
    }
}