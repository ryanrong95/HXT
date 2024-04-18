using System.Web;
using Yahv;
using Yahv.Utils.Serializers;

namespace WebApp.Api
{
    /// <summary>
    /// Leagues 的摘要说明
    /// </summary>
    public class Leagues : Yahv.Web.Handlers.JsonHandler
    {
        protected override void OnProcessRequest(HttpContext context)
        {
            string json = Erp.Current.Leagues.Json();
            context.Response.Write(json);
        }
    }
}