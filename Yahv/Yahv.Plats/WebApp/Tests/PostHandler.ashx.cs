using System.Web;

namespace WebApp.Tests
{
    /// <summary>
    /// PostHandler 的摘要说明
    /// </summary>
    public class PostHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}