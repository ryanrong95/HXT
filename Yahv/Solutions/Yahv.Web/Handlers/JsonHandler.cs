using System.Web;
using System.Web.SessionState;

namespace Yahv.Web.Handlers
{
    abstract public class JsonHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            this.OnProcessRequest(context);
        }

        abstract protected void OnProcessRequest(HttpContext context);

        virtual public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
