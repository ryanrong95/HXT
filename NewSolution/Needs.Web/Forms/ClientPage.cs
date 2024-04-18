using Needs.Utils.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace Needs.Web.Forms
{
    /// <summary>
    /// 原先base
    /// </summary>
    abstract public class ClientPage : System.Web.UI.Page
    {
        protected dynamic Model { get; set; } = new ExpandoObject();

        protected ClientPage()
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
                    Response.ContentType = "text/plain";
                    Response.Write(content as string);
                }
                else if (content is HtmlString)
                {
                    Response.ContentType = "text/html";
                    Response.Write(content as string);
                }
                else
                {
                    Response.ContentType = "application/json";
                    Response.Write(content.Json());
                }
            }

            Response.End();
        }

        protected void Alert(string message)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), new EasyuiAlert
            {
                Context = message,
                Title = "提示"
            }.Execute(), true);
        }
        protected void Alert(string message, Uri url, bool closed = false, bool isTopRedirect = false)
        {
            this.Alert(message, url.OriginalString, closed, isTopRedirect);
        }
        protected void Alert(string message, string url, bool closed = false, bool isTopRedirect = false)
        {
            this.Alert("提示", message, url, closed, isTopRedirect);
        }
        protected void Alert(string title, string message, string url, bool closed = false, bool isTopRedirect = false)
        {
            Response.ClearContent();
            Response.Write(new EasyuiAlert
            {
                Context = message,
                Title = title,
                Url = url,
                Closed = closed,
                IsTopRedirect = isTopRedirect
            }.Execute());
            Response.End();
        }

        /// <summary>
        /// 强制客户端跳转
        /// </summary>
        /// <param name="url"></param>
        protected void Redirect(string url, bool isTopRedirect = false)
        {
            Response.ClearContent();
            Response.Write(new Redirect
            {
                Url = url,
                IsTopRedirect = isTopRedirect
            }.Execute());
            Response.End();
        }

        

    }
}