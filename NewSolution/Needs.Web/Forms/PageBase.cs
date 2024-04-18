using Needs.Utils.Serializers;
using System;
using System.Dynamic;
using System.Reflection;


namespace Needs.Web.Forms
{
    public class BasePage : System.Web.UI.Page
    {
        protected dynamic Model { get; set; } = new ExpandoObject();

        public BasePage()
        {
            this.PreInit += Page_Init_Ajax;
        }

        protected void Page_Init_Ajax(object sender, EventArgs e)
        {
            string name = Request["action"];

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            MethodInfo minfo = this.GetType().GetMethod(name, BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase);

            if (minfo == null)
            {
                throw new NotImplementedException("No implementation of the Ajax method !");
            }

            Response.ClearContent();

            object content = minfo.Invoke(this, null);

            if (content != null)
            {
                if (content is string)
                {
                    Response.Write(content as string);
                }
                else
                {
                    Response.Write(content.Json());
                }
            }

            Response.End();
        }

        /// <summary>
        /// 强制客户端跳转
        /// </summary>
        /// <param name="url"></param>
        protected void Redirect(string url)
        {
            //Response.ClearContent();
            //string context = RedirectReader.Current.Html;
            //context = context.Replace("[url]", url);
            //Response.Write(context);
            //Response.End();
        }

        
    }
}