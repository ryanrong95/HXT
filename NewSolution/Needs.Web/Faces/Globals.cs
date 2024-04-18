using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Needs.Web.Faces
{
    public class Globals : HttpApplication
    {
        public Globals()
        {
            this.BeginRequest += Globals_BeginRequest;
            this.PreSendRequestHeaders += Globals_PreSendRequestHeaders;
        }

       

        private void Globals_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null
                || application.Request == null
                || application.Context == null
                || application.Context.Response == null)
            {
                return;
            }

            if (this.IsHttps)
            {
                if (application.Request.Url.Scheme == "http")
                {
                    application.Response.RedirectPermanent(application.Request.Url.AbsoluteUri.Replace("://", "s://"), true);
                }
            }


            if (Request.HttpMethod == "GET" || Request.HttpMethod == "POST")
            {
                var dtf = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                FieldInfo field = typeof(DateTimeFormatInfo).GetField("m_isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    throw new ArgumentException("请重新找寻修改只读的参数", "m_isReadOnly");
                }
                field.SetValue(dtf, false);

                dtf.DateSeparator = "-";
                dtf.LongTimePattern = "HH:mm:ss";
                dtf.ShortDatePattern = "yyyy-MM-dd";
            }

            if (Request.HttpMethod == "POST" && Request.Form.Count > 0)
            {
                var form = Request.Form;
                FieldInfo field = typeof(NameObjectCollectionBase).GetField("_readOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    throw new ArgumentException("请重新找寻修改只读的参数", "_readOnly");
                }
                field.SetValue(form, false);
                Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                for (int index = 0; index < form.AllKeys.Length; index++)
                {

                    string name = form.AllKeys[index];

                    if (name.Contains("Html"))
                    {
                        continue;
                    }

                    string value = form[index];

                    Request.Form[name] = Server.HtmlEncode(regex.Replace(value, " "));
                }
            }
        }

        private void Globals_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null
                || application.Request == null
                || application.Context == null
                || application.Context.Response == null)
            {
                return;
            }

            NameValueCollection headers = application.Context.Response.Headers;

            if (headers == null)
            {
                return;
            }

            headers.Remove("Server");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-Powered-By");
            /*
            HTTP/1.1 200 OK
            Cache-Control: private
            Content-Type: text/html; charset=utf-8
            Server: c07 testing
            X-Powered-By: ASP.NET
            Date: Thu, 20 Oct 2016 03:50:21 GMT
            Content-Length: 18310
            */

            headers.Set("Project Leader", "cute chen");
            headers.Set("Server", "ch gread");
        }

        protected virtual bool IsHttps
        {
            get
            {
                return false;
            }
        }

    }
}
